namespace ToDoList.Application.Common.Errors;

public class TaskItemErrors
{
    public static readonly Error TaskItemNotFound =
        Error.NotFound("TaskItems.TaskItemNotFound",
            "Task item not found.");

    public static readonly Error TaskItemUpdateFailed =
        Error.Conflict("TaskItems.TaskItemUpdateFailed",
            "Failed to update task item.");
}