using MediatR;
using Microsoft.Extensions.Logging;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Projects.Models;
using ToDoList.Application.Feature.Projects.Queries;
using ToDoList.Application.Repositories;

namespace ToDoList.Application.Feature.Projects.Handlers;

public class ListProjectsByCurrentUserQueryHandler(
    ILogger<ListProjectsByCurrentUserQueryHandler> logger,
    IProjectRepository projectRepository,
    ICurrentUserService currentUserService
) : IRequestHandler<ListProjectsByCurrentUserQuery, Result<IReadOnlyCollection<ProjectResponseModel>>>
{
    public async Task<Result<IReadOnlyCollection<ProjectResponseModel>>> Handle(ListProjectsByCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var projects = await projectRepository.GetAllAsync(
            userId,
            cancellationToken);

        logger.LogInformation(
            "User {UserId} retrieved {ProjectCount} projects",
            userId,
            projects.Count);

        return Result<IReadOnlyCollection<ProjectResponseModel>>.Success(projects);
    }
}