using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.Domain.Entities;

namespace ToDoList.Infrastructure.Persistence.EntityConfigurations;

public class ProjectEntityConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Color)
            .HasMaxLength(50);

        builder.Property(x => x.IsSystem)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.Name })
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Projects)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}