namespace ToDoList.Application.Feature.Projects.Models;

public record UpdateProjectModel(
    string Name,
    string? Description,
    string? Color);