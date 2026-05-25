using MediatR;
using Microsoft.Extensions.Logging;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.TaskItems.Commands;
using ToDoList.Application.Feature.TaskItems.Models;
using ToDoList.Application.Repositories;
using ToDoList.Domain.Entities;

namespace ToDoList.Application.Feature.TaskItems.Handlers;

public class CreateTaskItemCommandHandler(
    ILogger<CreateTaskItemCommandHandler> logger,
    ITaskItemRepository taskItemRepository,
    ICurrentUserService currentUserService,
    IProjectRepository projectRepository
) : IRequestHandler<CreateTaskItemCommand, Result<TaskItemResponseModel>>
{
    public async Task<Result<TaskItemResponseModel>> Handle(
        CreateTaskItemCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        Guid projectId;

        if (request.ProjectId.HasValue)
        {
            var project = await projectRepository.GetByIdAsync(
                request.ProjectId.Value,
                userId,
                cancellationToken);

            if (project is null)
            {
                logger.LogWarning(
                    "User {UserId} attempted to create task in project {ProjectId}, but project was not found",
                    userId,
                    request.ProjectId.Value);

                return Result<TaskItemResponseModel>.Failure(ProjectErrors.ProjectNotFound);
            }

            projectId = project.Id;
        }
        else
        {
            var systemProjectId = await projectRepository.GetSystemProjectIdAsync(
                userId,
                cancellationToken);

            if (!systemProjectId.HasValue)
            {
                logger.LogError(
                    "System project was not found for user {UserId} while creating task",
                    userId);

                return Result<TaskItemResponseModel>.Failure(ProjectErrors.ProjectNotFound);
            }

            projectId = systemProjectId.Value;
        }

        var taskItem = new TaskItem(
            projectId,
            request.Title,
            request.Description,
            request.Priority,
            request.DeadlineUtc);

        await taskItemRepository.CreateAsync(taskItem, cancellationToken);

        var createdTask = await taskItemRepository.GetByIdAsync(
            taskItem.Id,
            userId,
            cancellationToken);

        if (createdTask is null)
        {
            logger.LogError(
                "Task {TaskId} was created but could not be retrieved for user {UserId}",
                taskItem.Id,
                userId);

            return Result<TaskItemResponseModel>.Failure(TaskItemErrors.TaskItemNotFound);
        }

        logger.LogInformation(
            "User {UserId} created task {TaskId} in project {ProjectId}",
            userId,
            taskItem.Id,
            projectId);

        return Result<TaskItemResponseModel>.Success(createdTask);
    }
}