using ToDoList.Domain.Enums;

namespace ToDoList.Application.Feature.TaskItems.Models;

public record TaskItemResponseModel(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskPriority? Priority,
    DateTime? DeadlineUtc);