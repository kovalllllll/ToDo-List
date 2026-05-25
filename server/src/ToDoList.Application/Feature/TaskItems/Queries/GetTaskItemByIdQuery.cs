using MediatR;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.TaskItems.Models;

namespace ToDoList.Application.Feature.TaskItems.Queries;

public record GetTaskItemByIdQuery(Guid TaskId) : IRequest<Result<TaskItemResponseModel>>;