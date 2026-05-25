using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToDoList.API.Endpoints.Routes;
using ToDoList.API.Extensions;
using ToDoList.API.Requests;
using ToDoList.Application.Feature.Projects.Commands;
using ToDoList.Application.Feature.Projects.Models;
using ToDoList.Application.Feature.Projects.Queries;
using ToDoList.Domain.Enums;

namespace ToDoList.API.Endpoints;

public static class ProjectEndpointExtensions
{
    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Projects.Create,
                async (CreateProjectCommand command, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(command, cancellationToken);

                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
            .WithTags("Projects");

        app.MapPatch(ApiRoutes.Projects.Update,
                async (Guid projectId, UpdateProjectModel request, ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateProjectCommand(projectId, request.Name, request.Description, request.Color);
                    var result = await sender.Send(command, cancellationToken);

                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
            .WithTags("Projects");

        app.MapDelete(ApiRoutes.Projects.Delete,
                async (Guid projectId, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new DeleteProjectCommand(projectId), cancellationToken);

                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
            .WithTags("Projects");

        app.MapGet(ApiRoutes.Projects.GetAll,
                async (ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new ListProjectsByCurrentUserQuery(), cancellationToken);

                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
            .WithTags("Projects");

        app.MapGet(ApiRoutes.Projects.GetAllTasksByProjectId,
                async (
                    Guid projectId,
                    GetProjectTasksRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var filter = new ProjectTasksFilterModel(
                        request.Search,
                        request.Statuses,
                        request.Priorities);

                    var result = await sender.Send(
                        new GetProjectTasksQuery(projectId, filter),
                        cancellationToken);

                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
            .WithTags("Projects");

        app.MapGet(ApiRoutes.Projects.GetById,
                async (Guid projectId, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new GetProjectByIdQuery(projectId), cancellationToken);

                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
            .WithTags("Projects");

        return app;
    }
}