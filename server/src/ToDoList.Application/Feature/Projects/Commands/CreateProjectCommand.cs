using MediatR;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Projects.Models;

namespace ToDoList.Application.Feature.Projects.Commands;

public record CreateProjectCommand(string Name, string? Description = null, string? Color = null)
    : IRequest<Result<ProjectResponseModel>>;