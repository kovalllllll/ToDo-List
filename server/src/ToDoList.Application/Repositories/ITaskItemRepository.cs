using ToDoList.Application.Feature.TaskItems.Models;
using ToDoList.Domain.Entities;
using ToDoList.Domain.Enums;

namespace ToDoList.Application.Repositories;

public interface ITaskItemRepository
{
    Task CreateAsync(TaskItem taskItem, CancellationToken cancellationToken);

    Task<TaskItemResponseModel?> GetByIdAsync(
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<bool> UpdateAsync(
        Guid taskId,
        Guid userId,
        string title,
        string? description,
        TaskItemStatus status,
        TaskPriority? priority,
        DateTime? deadlineUtc,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken);
}