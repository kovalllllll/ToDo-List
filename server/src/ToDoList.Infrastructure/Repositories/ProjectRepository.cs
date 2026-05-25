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
                           INSERT INTO Projects (Id, UserId, Name, Description, Color, IsSystem, CreatedAtUtc)
                           VALUES (@Id, @UserId, @Name, @Description, @Color, @IsSystem, @CreatedAtUtc);
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
                project.IsSystem,
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
                               IsSystem,
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
            project.IsSystem,
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
                               IsSystem,
                               CreatedAtUtc
                           FROM Projects
                           WHERE UserId = @UserId
                           ORDER BY IsSystem DESC, CreatedAtUtc DESC;

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
                project.IsSystem,
                project.CreatedAtUtc,
                tasksByProjectId.GetValueOrDefault(project.Id, Array.Empty<TaskItemResponseModel>())))
            .ToList();
    }

    public async Task<Guid?> GetSystemProjectIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT TOP 1 Id
                           FROM Projects
                           WHERE UserId = @UserId
                             AND IsSystem = 1
                           ORDER BY CreatedAtUtc;
                           """;

        using var connection = connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<Guid?>(new CommandDefinition(
            sql,
            new { UserId = userId },
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<TaskItemResponseModel>> GetAllTasksByProjectIdAsync(Guid projectId,
        Guid userId, ProjectTasksFilterModel filter,
        CancellationToken cancellationToken)
    {
        var sql = """
                  SELECT
                      ti.Id,
                      ti.ProjectId,
                      ti.Title,
                      ti.Description,
                      ti.Status,
                      ti.Priority,
                      ti.DeadlineUtc
                  FROM TaskItems ti
                  INNER JOIN Projects p ON p.Id = ti.ProjectId
                  WHERE ti.ProjectId = @ProjectId
                    AND p.UserId = @UserId
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("ProjectId", projectId);
        parameters.Add("UserId", userId);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            sql += """

                     AND (
                         ti.Title LIKE @Search
                         OR ti.Description LIKE @Search
                     )
                   """;

            parameters.Add("Search", $"%{filter.Search}%");
        }

        if (filter.Statuses is { Count: > 0 })
        {
            sql += """

                     AND ti.Status IN @Statuses
                   """;

            parameters.Add("Statuses", filter.Statuses);
        }

        if (filter.Priorities is { Count: > 0 })
        {
            sql += """

                     AND ti.Priority IN @Priorities
                   """;

            parameters.Add("Priorities", filter.Priorities);
        }

        sql += """

               ORDER BY ti.DeadlineUtc, ti.Title
               """;

        using var connection = connectionFactory.CreateConnection();

        var tasks = await connection.QueryAsync<TaskItemResponseModel>(
            new CommandDefinition(
                sql,
                parameters,
                cancellationToken: cancellationToken));

        return tasks.AsList();
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