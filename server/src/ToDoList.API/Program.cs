using System.Text.Json;
using System.Text.Json.Serialization;
using Scalar.AspNetCore;
using Serilog;
using ToDoList.Application;
using ToDoList.API.Endpoints;
using ToDoList.API.Extensions;
using ToDoList.API.Middleware;
using ToDoList.Infrastructure;

namespace ToDoList.API;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((ctx, config) =>
            config.ReadFrom.Configuration(ctx.Configuration));

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularDev", policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:4200",
                        "http://localhost:4201",
                        "http://localhost:4202")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // Add services to the container.
        builder.Services.AddAuthorization()
            .AddHttpContextAccessor()
            .AddEndpointsApiExplorer();

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration)
            .AddAuth(builder.Configuration);

        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddNativeOpenApi();

        var app = builder.Build();

        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();

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

        // CORS must be before Authentication/Authorization
        app.UseCors("AllowAngularDev");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapUserEndpoints()
            .MapProjectEndpoints()
            .MapTaskItemEndpoints();

        app.Run();
    }
}