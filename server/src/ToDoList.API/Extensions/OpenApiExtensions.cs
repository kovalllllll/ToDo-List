using Microsoft.OpenApi;
using ToDoList.API.Security;

namespace ToDoList.API.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddNativeOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "ToDoList API",
                    Version = "v1",
                    Description = "API for managing projects and tasks in a ToDo list application.",
                    Contact = new OpenApiContact
                    {
                        Name = "kovall",
                        Email = "kovalartem901@gmail.com",
                        Url = new Uri("https://github.com/kovalllllll")
                    }
                };

                return Task.CompletedTask;
            });

            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        return services;
    }
}