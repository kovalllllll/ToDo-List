using Dapper;
using ToDoList.Application.Feature.Projects.Models;
using ToDoList.Application.Feature.TaskItems.Models;
using ToDoList.Application.Repositories;
using ToDoList.Domain.Entities;
using ToDoList.Infrastructure.Data;

namespace ToDoList.Infrastructure.Repositories;

public class ProjectRepository(IDbConnectionFactory connectionFactory) : IProjectRepository
{
    public async Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT CAST(CASE
                               WHEN EXISTS (
                                   SELECT 1
                                   FROM Projects
                                   WHERE UserId = @UserId AND Name = @Name
                               )
                               THEN 1 ELSE 0
                           END AS BIT)
                           """;

        using var connection = connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            sql,
            new { UserId = userId, Name = name },
            cancellationToken: cancellationToken));
    }

    public async Task CreateAsync(Project project, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO Projects (Id, UserId, Name, Description, Color, CreatedAtUtc)
                           VALUES (@Id, @UserId, @Name, @Description, @Color, @CreatedAtUtc);
                           """;

        using var connection = connectionFactory.CreateConnection();

        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new
            {
                project.Id,
                project.UserId,
                project.Name,
                project.Description,
                project.Color,
                project.CreatedAtUtc
            },
            cancellationToken: cancellationToken));
    }

    public async Task<ProjectResponseModel?> GetByIdAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT
                               Id,
                               UserId,
                               Name,
                               Description,
                               Color,
                               CreatedAtUtc
                           FROM Projects
                           WHERE Id = @ProjectId AND UserId = @UserId;

                           SELECT
                               Id,
                               ProjectId,
                               Title,
                               Description,
                               Status,
                               Priority,
                               DeadlineUtc
                           FROM TaskItems
                           WHERE ProjectId = @ProjectId
                           ORDER BY DeadlineUtc, Title;
                           """;

        using var connection = connectionFactory.CreateConnection();

        await using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            new
            {
                ProjectId = projectId,
                UserId = userId
            },
            cancellationToken: cancellationToken));

        var project = await multi.ReadFirstOrDefaultAsync<ProjectModel>();

        if (project is null)
        {
            return null;
        }

        var tasks = (await multi.ReadAsync<TaskItemResponseModel>()).ToList();

        return new ProjectResponseModel(
            project.Id,
            project.UserId,
            project.Name,
            project.Description,
            project.Color,
            project.CreatedAtUtc,
            tasks);
    }

    public async Task<IReadOnlyCollection<ProjectResponseModel>> GetAllAsync(Guid userId,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT
                               Id,
                               UserId,
                               Name,
                               Description,
                               Color,
                               CreatedAtUtc
                           FROM Projects
                           WHERE UserId = @UserId
                           ORDER BY CreatedAtUtc DESC;

                           SELECT
                               t.Id,
                               t.ProjectId,
                               t.Title,
                               t.Description,
                               t.Status,
                               t.Priority,
                               t.DeadlineUtc
                           FROM TaskItems t
                           INNER JOIN Projects p ON p.Id = t.ProjectId
                           WHERE p.UserId = @UserId
                           ORDER BY t.DeadlineUtc, t.Title;
                           """;

        using var connection = connectionFactory.CreateConnection();

        await using var multi = await connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            new { UserId = userId },
            cancellationToken: cancellationToken));

        var projects = (await multi.ReadAsync<ProjectModel>()).ToList();
        var tasks = (await multi.ReadAsync<TaskItemResponseModel>()).ToList();

        var tasksByProjectId = tasks
            .GroupBy(t => t.ProjectId)
            .ToDictionary(g => g.Key, g => (IReadOnlyCollection<TaskItemResponseModel>)g.ToList());

        return projects
            .Select(project => new ProjectResponseModel(
                project.Id,
                project.UserId,
                project.Name,
                project.Description,
                project.Color,
                project.CreatedAtUtc,
                tasksByProjectId.GetValueOrDefault(project.Id, Array.Empty<TaskItemResponseModel>())))
            .ToList();
    }

    public async Task<bool> UpdateAsync(
        Guid projectId,
        Guid userId,
        string name,
        string? description,
        string? color,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           UPDATE Projects
                           SET
                               Name = @Name,
                               Description = @Description,
                               Color = @Color
                           WHERE Id = @ProjectId
                             AND UserId = @UserId;
                           """;

        using var connection = connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new
            {
                ProjectId = projectId,
                UserId = userId,
                Name = name,
                Description = description,
                Color = color
            },
            cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           DELETE FROM Projects
                           WHERE Id = @ProjectId AND UserId = @UserId
                           """;

        using var connection = connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { ProjectId = projectId, UserId = userId },
            cancellationToken: cancellationToken));

        return affectedRows > 0;
    }
}