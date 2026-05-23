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
        var deleted = await projectRepository.DeleteAsync(
            request.ProjectId,
            currentUserService.UserId,
            cancellationToken);

        if (!deleted)
        {
            logger.LogWarning(
                "User {UserId} attempted to delete project {ProjectId}, but it was not found",
                currentUserService.UserId,
                request.ProjectId);

            return Result<bool>.Failure(ProjectErrors.ProjectNotFound);
        }

        logger.LogInformation(
            "User {UserId} deleted project {ProjectId}",
            currentUserService.UserId,
            request.ProjectId);

        return Result<bool>.Success(true);
    }
}