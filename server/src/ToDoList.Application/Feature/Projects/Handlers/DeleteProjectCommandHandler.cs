using MediatR;
using Microsoft.Extensions.Logging;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Projects.Commands;
using ToDoList.Application.Repositories;

namespace ToDoList.Application.Feature.Projects.Handlers;

public sealed class DeleteProjectCommandHandler(
    ILogger<DeleteProjectCommandHandler> logger,
    IProjectRepository projectRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<DeleteProjectCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteProjectCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var project = await projectRepository.GetByIdAsync(
            request.ProjectId,
            userId,
            cancellationToken);

        if (project is null)
        {
            logger.LogWarning(
                "User {UserId} attempted to delete project {ProjectId}, but it was not found",
                userId,
                request.ProjectId);

            return Result<bool>.Failure(ProjectErrors.ProjectNotFound);
        }

        if (project.IsSystem)
        {
            logger.LogWarning(
                "User {UserId} attempted to delete system project {ProjectId}",
                userId,
                request.ProjectId);

            return Result<bool>.Failure(ProjectErrors.SystemProjectCannotBeDeleted);
        }

        var deleted = await projectRepository.DeleteAsync(
            request.ProjectId,
            userId,
            cancellationToken);

        if (!deleted)
        {
            logger.LogWarning(
                "User {UserId} attempted to delete project {ProjectId}, but deletion failed",
                userId,
                request.ProjectId);

            return Result<bool>.Failure(ProjectErrors.ProjectNotFound);
        }

        logger.LogInformation(
            "User {UserId} deleted project {ProjectId}",
            userId,
            request.ProjectId);

        return Result<bool>.Success(true);
    }
}