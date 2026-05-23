using MediatR;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Projects.Models;

namespace ToDoList.Application.Feature.Projects.Commands;

public sealed record UpdateProjectCommand(
    Guid ProjectId,
    string Name,
    string? Description,
    string? Color)
    : IRequest<Result<ProjectResponseModel>>;