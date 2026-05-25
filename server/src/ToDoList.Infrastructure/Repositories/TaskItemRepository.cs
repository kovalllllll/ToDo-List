using Dapper;
using ToDoList.Application.Feature.TaskItems.Models;
using ToDoList.Application.Repositories;
using ToDoList.Domain.Entities;
using ToDoList.Domain.Enums;
using ToDoList.Infrastructure.Data;

namespace ToDoList.Infrastructure.Repositories;

public class TaskItemRepository(IDbConnectionFactory connectionFactory) : ITaskItemRepository
{
    public async Task CreateAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO TaskItems (Id, ProjectId, Title, Description, Status, Priority, DeadlineUtc)
                           VALUES (@Id, @ProjectId, @Title, @Description, @Status, @Priority, @DeadlineUtc);
                           """;

        using var connection = connectionFactory.CreateConnection();

        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new
            {
                taskItem.Id,
                taskItem.ProjectId,
                taskItem.Title,
                taskItem.Description,
                taskItem.Status,
                taskItem.Priority,
                taskItem.DeadlineUtc
            },
            cancellationToken: cancellationToken));
    }

    public async Task<TaskItemResponseModel?> GetByIdAsync(Guid taskId, Guid userId,
        CancellationToken cancellationToken)
    {
        const string sql = """
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
                           WHERE ti.Id = @TaskId AND p.UserId = @UserId;
                           """;

        using var connection = connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<TaskItemResponseModel>(
            new CommandDefinition(
                sql,
                new { TaskId = taskId, UserId = userId },
                cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(
        Guid taskId,
        Guid userId,
        string title,
        string? description,
        TaskItemStatus status,
        TaskPriority? priority,
        DateTime? deadlineUtc,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           UPDATE ti
                           SET Title = @Title,
                               Description = @Description,
                               Status = @Status,
                               Priority = @Priority,
                               DeadlineUtc = @DeadlineUtc
                           FROM TaskItems ti
                           INNER JOIN Projects p ON p.Id = ti.ProjectId
                           WHERE ti.Id = @TaskId AND p.UserId = @UserId;
                           """;

        using var connection = connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new
            {
                TaskId = taskId,
                UserId = userId,
                Title = title,
                Description = description,
                Status = status,
                Priority = priority,
                DeadlineUtc = deadlineUtc
            },
            cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(Guid taskId, Guid userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           DELETE ti
                           FROM TaskItems ti
                           INNER JOIN Projects p ON p.Id = ti.ProjectId
                           WHERE ti.Id = @TaskId AND p.UserId = @UserId;
                           """;

        using var connection = connectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { TaskId = taskId, UserId = userId },
            cancellationToken: cancellationToken));

        return affectedRows > 0;
    }
}