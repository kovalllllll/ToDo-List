namespace ToDoList.Domain.Entities;

public sealed class Project
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Color { get; private set; }
    public bool IsSystem { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public ApplicationUser User { get; private set; } = null!;
    public ICollection<TaskItem> Tasks { get; private set; } = new List<TaskItem>();

    public Project(Guid userId, string name, string? description = null, string? color = null, bool isSystem = false)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        Description = description;
        Color = color;
        IsSystem = isSystem;
        CreatedAtUtc = DateTime.UtcNow;
    }
}