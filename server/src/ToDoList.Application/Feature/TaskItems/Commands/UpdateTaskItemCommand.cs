using MediatR;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.TaskItems.Models;
using ToDoList.Domain.Enums;

namespace ToDoList.Application.Feature.TaskItems.Commands;

public record UpdateTaskItemCommand(
    Guid Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskPriority? Priority,
    DateTime? DeadlineUtc) : IRequest<Result<TaskItemResponseModel>>;