using MediatR;
using Microsoft.Extensions.Logging;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.TaskItems.Commands;
using ToDoList.Application.Repositories;

namespace ToDoList.Application.Feature.TaskItems.Handlers;

public class DeleteTaskItemCommandHandler(
    ILogger<DeleteTaskItemCommandHandler> logger,
    ICurrentUserService currentUserService,
    ITaskItemRepository taskItemRepository
)
    : IRequestHandler<DeleteTaskItemCommand, Result>
{
    public async Task<Result> Handle(DeleteTaskItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var deleted = await taskItemRepository.DeleteAsync(request.Id, userId, cancellationToken);

        if (!deleted)
        {
            logger.LogWarning(
                "User {UserId} attempted to delete task item with id {TaskItemId}, but it was not found",
                userId,
                request.Id);

            return Result.Failure(TaskItemErrors.TaskItemNotFound);
        }

        logger.LogInformation(
            "User {UserId} deleted task item with id {TaskItemId}",
            userId,
            request.Id);

        return Result.Success();
    }
}