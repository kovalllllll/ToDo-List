using ToDoList.Domain.Enums;

namespace ToDoList.API.Requests;

public class GetProjectTasksRequest
{
    public string? Search { get; init; }
    public List<TaskItemStatus>? Statuses { get; init; }
    public List<TaskPriority>? Priorities { get; init; }

    public static ValueTask<GetProjectTasksRequest?> BindAsync(HttpContext context)
    {
        var query = context.Request.Query;

        var model = new GetProjectTasksRequest
        {
            Search = query["search"].FirstOrDefault(),
            Statuses = query["statuses"]
                .Select(x => Enum.TryParse<TaskItemStatus>(x, true, out var s) ? s : (TaskItemStatus?)null)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToList(),
            Priorities = query["priorities"]
                .Select(x => Enum.TryParse<TaskPriority>(x, true, out var p) ? p : (TaskPriority?)null)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToList()
        };

        return ValueTask.FromResult<GetProjectTasksRequest?>(model);
    }
}