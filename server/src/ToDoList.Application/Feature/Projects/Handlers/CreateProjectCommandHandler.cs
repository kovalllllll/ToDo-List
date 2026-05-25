using MediatR;
using Microsoft.Extensions.Logging;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Projects.Commands;
using ToDoList.Application.Feature.Projects.Models;
using ToDoList.Application.Repositories;
using ToDoList.Domain.Entities;

namespace ToDoList.Application.Feature.Projects.Handlers;

public class CreateProjectCommandHandler(
    ILogger<CreateProjectCommandHandler> logger,
    IProjectRepository projectRepository,
    ICurrentUserService currentUserService
) : IRequestHandler<CreateProjectCommand, Result<ProjectResponseModel>>
{
    public async Task<Result<ProjectResponseModel>> Handle(CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var exist =
            await projectRepository.ExistsByNameAsync(currentUserService.UserId, request.Name, cancellationToken);

        if (exist)
        {
            logger.LogWarning("User {UserId} attempted to create a project with a duplicate name: {ProjectName}",
                currentUserService.UserId, request.Name);

            return Result<ProjectResponseModel>.Failure(ProjectErrors.ProjectAlreadyExists);
        }

        var project = new Project(
            currentUserService.UserId,
            request.Name,
            request.Description,
            request.Color);

        await projectRepository.CreateAsync(project, cancellationToken);

        logger.LogInformation("User {UserId} created a new project: {ProjectName} (ID: {ProjectId})",
            currentUserService.UserId, project.Name, project.Id);

        var response = new ProjectResponseModel(
            project.Id,
            project.UserId,
            project.Name,
            project.Description,
            project.Color,
            project.IsSystem,
            project.CreatedAtUtc,
            []);

        return Result<ProjectResponseModel>.Success(response);
    }
}