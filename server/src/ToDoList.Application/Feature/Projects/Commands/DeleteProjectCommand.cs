using MediatR;
using ToDoList.Application.Common.Results;

namespace ToDoList.Application.Feature.Projects.Commands;

public record DeleteProjectCommand(Guid ProjectId) : IRequest<Result>;