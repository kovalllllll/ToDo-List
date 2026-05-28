using MediatR;
using ToDoList.API.Endpoints.Routes;
using ToDoList.API.Extensions;
using ToDoList.Application.Feature.Users.Commands;

namespace ToDoList.API.Endpoints;

public static class UserEndpointExtensions
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Users.SignUp,
            async (SignUpCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result.ToMinimalApiResult();
            }).WithTags("Users");

        app.MapPost(ApiRoutes.Users.SignIn,
                async (SignInCommand command, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(command, cancellationToken);

                    return result.ToMinimalApiResult();
                })
            .WithTags("Users");

        return app;
    }
}