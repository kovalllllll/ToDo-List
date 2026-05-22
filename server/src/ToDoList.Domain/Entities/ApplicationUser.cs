using Microsoft.AspNetCore.Identity;

namespace ToDoList.Domain.Entities;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public DateTime CreatedAtUtc { get; private set; }

    public ICollection<Project> Projects { get; private set; } = new List<Project>();

    private ApplicationUser()
    {
    }

    public ApplicationUser(string email, string userName)
    {
        Email = email;
        UserName = userName;
        CreatedAtUtc = DateTime.UtcNow;
    }
}