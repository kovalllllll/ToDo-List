using MediatR;
using ToDoList.API.Endpoints.Routes;
using ToDoList.API.Extensions;
using ToDoList.Application.Feature.TaskItems.Commands;
using ToDoList.Application.Feature.TaskItems.Models;
using ToDoList.Application.Feature.TaskItems.Queries;

namespace ToDoList.API.Endpoints;

public static class TaskItemEndpointExtensions
{
    public static IEndpointRouteBuilder MapTaskItemEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Tasks.Create,
                async (CreateTaskItemCommand command, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(command, cancellationToken);

                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
            .WithTags("Tasks");

        app.MapPatch(ApiRoutes.Tasks.Update,
                async (
                    Guid taskId,
                    UpdateTaskItemModel request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateTaskItemCommand(
                        taskId,
                        request.Title,
                        request.Description,
                        request.Status,
                        request.Priority,
                        request.DeadlineUtc);

                    var result = await sender.Send(command, cancellationToken);

                    return result.ToMinimalApiResult();
                })
            .RequireAuthorization()
            .WithTags("Tasks");

        app.MapDelete(ApiRoutes.Tasks.Delete,
                async (Guid taskId, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new DeleteTaskItemCommand(taskId), cancellationToken);

                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
            .WithTags("Tasks");

        app.MapGet(ApiRoutes.Tasks.GetById,
                async (Guid taskId, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new GetTaskItemByIdQuery(taskId), cancellationToken);

                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
            .WithTags("Tasks");

        return app;
    }
}