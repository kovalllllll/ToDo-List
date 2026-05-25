using MediatR;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Projects.Models;

namespace ToDoList.Application.Feature.Projects.Queries;

public sealed record ListProjectsByCurrentUserQuery
    : IRequest<Result<IReadOnlyCollection<ProjectResponseModel>>>;