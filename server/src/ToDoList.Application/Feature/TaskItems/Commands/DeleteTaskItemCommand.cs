using MediatR;
using ToDoList.Application.Common.Results;

namespace ToDoList.Application.Feature.TaskItems.Commands;

public record DeleteTaskItemCommand(
    Guid Id) : IRequest<Result>;