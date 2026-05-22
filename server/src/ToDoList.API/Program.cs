using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using ToDoList.Application;
using ToDoList.API.Endpoints;
using ToDoList.API.Extensions;
using ToDoList.API.Middleware;
using ToDoList.API.Security;
using ToDoList.Infrastructure;

namespace ToDoList.API;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddAuthorizationBuilder();

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddAuth(builder.Configuration);

        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddNativeOpenApi();

        var app = builder.Build();

        app.UseExceptionHandler();

        app.MapOpenApi();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("ToDoList API");
                options.DefaultHttpClient =
                    new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.HttpClient);
                options.AddPreferredSecuritySchemes("Bearer")
                    .AddHttpAuthentication("Bearer", auth =>
                    {
                        auth.Token = "";
                        auth.Description = "JWT Bearer token authentication";
                    }).EnablePersistentAuthentication();
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapUserEndpoints();

        app.Run();
    }
}
