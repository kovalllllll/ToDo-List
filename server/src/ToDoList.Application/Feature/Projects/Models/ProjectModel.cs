namespace ToDoList.Application.Feature.Projects.Models;

public sealed record ProjectModel(
    Guid Id,
    Guid UserId,
    string Name,
    string? Description,
    string? Color,
    bool IsSystem,
    DateTime CreatedAtUtc);