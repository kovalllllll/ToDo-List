using MediatR;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Projects.Models;

namespace ToDoList.Application.Feature.Projects.Queries;

public record GetProjectByIdQuery(Guid Id) : IRequest<Result<ProjectResponseModel>>;