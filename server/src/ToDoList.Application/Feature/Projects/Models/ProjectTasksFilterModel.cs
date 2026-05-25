using ToDoList.Domain.Enums;

namespace ToDoList.Application.Feature.Projects.Models;

public record ProjectTasksFilterModel(
    string? Search,
    List<TaskItemStatus>? Statuses,
    List<TaskPriority>? Priorities);