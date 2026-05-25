using MediatR;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Projects.Models;
using ToDoList.Application.Feature.TaskItems.Models;
using ToDoList.Domain.Enums;

namespace ToDoList.Application.Feature.Projects.Queries;

public record GetProjectTasksQuery(
    Guid ProjectId,
    ProjectTasksFilterModel Filter)
    : IRequest<Result<IReadOnlyCollection<TaskItemResponseModel>>>;