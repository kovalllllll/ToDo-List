using MediatR;
using Microsoft.Extensions.Logging;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Projects.Models;
using ToDoList.Application.Feature.Projects.Queries;
using ToDoList.Application.Repositories;

namespace ToDoList.Application.Feature.Projects.Handlers;

public class GetProjectByIdQueryHandler(
    ILogger<GetProjectByIdQueryHandler> logger,
    IProjectRepository projectRepository,
    ICurrentUserService currentUserService
) : IRequestHandler<GetProjectByIdQuery, Result<ProjectResponseModel>>
{
    public async Task<Result<ProjectResponseModel>> Handle(GetProjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var project = await projectRepository.GetByIdAsync(
            request.Id,
            userId,
            cancellationToken);

        if (project is null)
        {
            logger.LogWarning("Project with id {ProjectId} not found for user {UserId}", request.Id,
                currentUserService.UserId);

            return Result<ProjectResponseModel>.Failure(ProjectErrors.ProjectNotFound);
        }

        return Result<ProjectResponseModel>.Success(project);
    }
}