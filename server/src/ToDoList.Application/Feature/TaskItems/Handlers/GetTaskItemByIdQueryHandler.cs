using MediatR;
using Microsoft.Extensions.Logging;
using Serilog;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.TaskItems.Models;
using ToDoList.Application.Feature.TaskItems.Queries;
using ToDoList.Application.Repositories;

namespace ToDoList.Application.Feature.TaskItems.Handlers;

public class GetTaskItemByIdQueryHandler(
    ILogger<GetTaskItemByIdQueryHandler> logger,
    ITaskItemRepository taskItemRepository,
    ICurrentUserService currentUserService
) : IRequestHandler<GetTaskItemByIdQuery, Result<TaskItemResponseModel>>
{
    public async Task<Result<TaskItemResponseModel>> Handle(GetTaskItemByIdQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var task = await taskItemRepository.GetByIdAsync(request.TaskId, userId, cancellationToken);

        if (task is null)
        {
            logger.LogWarning("Task with id {TaskId} not found for user {UserId}", request.TaskId,
                currentUserService.UserId);

            return Result<TaskItemResponseModel>.Failure(TaskItemErrors.TaskItemNotFound);
        }

        return Result<TaskItemResponseModel>.Success(task);
    }
}