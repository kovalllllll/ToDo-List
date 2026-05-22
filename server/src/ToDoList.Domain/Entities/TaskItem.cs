using ToDoList.Domain.Enums;

namespace ToDoList.Domain.Entities;

public sealed class TaskItem
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }

    public string Title { get; private set; }
    public string? Description { get; private set; }

    public TaskItemStatus Status { get; private set; }
    public TaskPriority? Priority { get; private set; }

    public DateTime? DeadlineUtc { get; private set; }

    public Project Project { get; private set; } = null!;


    public TaskItem(
        Guid projectId,
        string title,
        string? description = null,
        TaskPriority? priority = null,
        DateTime? deadlineUtc = null)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        Title = title;
        Description = description;
        Priority = priority;
        DeadlineUtc = deadlineUtc;
        Status = TaskItemStatus.Todo;
    }
}