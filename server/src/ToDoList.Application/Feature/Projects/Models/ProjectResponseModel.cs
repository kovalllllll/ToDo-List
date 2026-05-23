using ToDoList.Application.Feature.TaskItems.Models;

namespace ToDoList.Application.Feature.Projects.Models;

public record ProjectResponseModel(
    Guid Id,
    Guid UserId,
    string Name,
    string? Description,
    string? Color,
    DateTime CreatedAt,
    IReadOnlyCollection<TaskItemResponseModel> Tasks
);