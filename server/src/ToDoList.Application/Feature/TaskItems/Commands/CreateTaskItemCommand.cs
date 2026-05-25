using MediatR;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.TaskItems.Models;
using ToDoList.Domain.Enums;

namespace ToDoList.Application.Feature.TaskItems.Commands;

public record CreateTaskItemCommand(
    Guid? ProjectId,
    string Title,
    string? Description,
    TaskPriority? Priority,
    DateTime? DeadlineUtc) : IRequest<Result<TaskItemResponseModel>>;