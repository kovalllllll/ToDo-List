namespace ToDoList.Application.Common.Errors;

public class ProjectErrors
{
    public static readonly Error ProjectNotFound =
        Error.NotFound("Projects.ProjectNotFound",
            "Project not found.");

    public static readonly Error ProjectAlreadyExists =
        Error.Conflict("Projects.ProjectAlreadyExists",
            "Project already exists.");
}