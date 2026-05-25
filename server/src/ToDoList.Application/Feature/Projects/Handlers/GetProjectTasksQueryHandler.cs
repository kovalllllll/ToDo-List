using MediatR;
using Microsoft.Extensions.Logging;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Projects.Queries;
using ToDoList.Application.Feature.TaskItems.Models;
using ToDoList.Application.Repositories;

namespace ToDoList.Application.Feature.Projects.Handlers;

public class GetProjectTasksQueryHandler(
    ILogger<GetProjectTasksQueryHandler> logger,
    ICurrentUserService currentUserService,
    IProjectRepository projectRepository)
    : IRequestHandler<GetProjectTasksQuery, Result<IReadOnlyCollection<TaskItemResponseModel>>>
{
    public async Task<Result<IReadOnlyCollection<TaskItemResponseModel>>> Handle(
        GetProjectTasksQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var tasks = await projectRepository.GetAllTasksByProjectIdAsync(
            request.ProjectId,
            userId,
            request.Filter,
            cancellationToken);

        logger.LogInformation(
            "Retrieved {TaskCount} tasks for project {ProjectId} and user {UserId}",
            tasks.Count,
            request.ProjectId,
            userId);

        return Result<IReadOnlyCollection<TaskItemResponseModel>>.Success(tasks);
    }
}