using MediatR;
using Microsoft.Extensions.Logging;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Projects.Commands;
using ToDoList.Application.Feature.Projects.Models;
using ToDoList.Application.Repositories;

namespace ToDoList.Application.Feature.Projects.Handlers;

public sealed class UpdateProjectCommandHandler(
    ILogger<UpdateProjectCommandHandler> logger,
    IProjectRepository projectRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateProjectCommand, Result<ProjectResponseModel>>
{
    public async Task<Result<ProjectResponseModel>> Handle(
        UpdateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var updated = await projectRepository.UpdateAsync(
            request.ProjectId,
            currentUserService.UserId,
            request.Name,
            request.Description,
            request.Color,
            cancellationToken);

        if (!updated)
        {
            logger.LogWarning(
                "User {UserId} attempted to update project {ProjectId}, but it was not found",
                currentUserService.UserId,
                request.ProjectId);

            return Result<ProjectResponseModel>.Failure(ProjectErrors.ProjectNotFound);
        }

        var project = await projectRepository.GetByIdAsync(
            request.ProjectId,
            currentUserService.UserId,
            cancellationToken);

        if (project is null)
        {
            logger.LogError(
                "Project {ProjectId} was updated but could not be retrieved for user {UserId}",
                request.ProjectId,
                currentUserService.UserId);

            return Result<ProjectResponseModel>.Failure(ProjectErrors.ProjectNotFound);
        }

        logger.LogInformation(
            "User {UserId} updated project {ProjectId}",
            currentUserService.UserId,
            request.ProjectId);

        return Result<ProjectResponseModel>.Success(project);
    }
}