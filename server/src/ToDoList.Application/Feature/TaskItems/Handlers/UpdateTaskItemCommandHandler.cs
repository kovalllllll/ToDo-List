using MediatR;
using Microsoft.Extensions.Logging;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.TaskItems.Commands;
using ToDoList.Application.Feature.TaskItems.Models;
using ToDoList.Application.Repositories;

namespace ToDoList.Application.Feature.TaskItems.Handlers;

public class UpdateTaskItemCommandHandler(
    ILogger<UpdateTaskItemCommandHandler> logger,
    ICurrentUserService currentUserService,
    ITaskItemRepository taskItemRepository)
    : IRequestHandler<UpdateTaskItemCommand, Result<TaskItemResponseModel>>
{
    public async Task<Result<TaskItemResponseModel>> Handle(
        UpdateTaskItemCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var existingTaskItem = await taskItemRepository.GetByIdAsync(
            request.Id,
            userId,
            cancellationToken);

        if (existingTaskItem is null)
        {
            logger.LogWarning(
                "User {UserId} attempted to update task item {TaskItemId}, but it was not found",
                userId,
                request.Id);

            return Result<TaskItemResponseModel>.Failure(TaskItemErrors.TaskItemNotFound);
        }

        var updated = await taskItemRepository.UpdateAsync(
            request.Id,
            userId,
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            request.DeadlineUtc,
            cancellationToken);

        if (!updated)
        {
            logger.LogError(
                "Failed to update task item {TaskItemId} for user {UserId}",
                request.Id,
                userId);

            return Result<TaskItemResponseModel>.Failure(TaskItemErrors.TaskItemUpdateFailed);
        }

        var updatedTaskItem = await taskItemRepository.GetByIdAsync(
            request.Id,
            userId,
            cancellationToken);

        if (updatedTaskItem is null)
        {
            logger.LogError(
                "Task item {TaskItemId} for user {UserId} was updated but could not be retrieved afterward",
                request.Id,
                userId);

            return Result<TaskItemResponseModel>.Failure(TaskItemErrors.TaskItemNotFound);
        }

        logger.LogInformation(
            "User {UserId} updated task item {TaskItemId}",
            userId,
            request.Id);

        return Result<TaskItemResponseModel>.Success(updatedTaskItem);
    }
}