namespace ToDoList.Application.Common.Errors;

public class ProjectErrors
{
    public static readonly Error ProjectNotFound =
        Error.NotFound("Projects.ProjectNotFound",
            "Project not found.");

    public static readonly Error ProjectAlreadyExists =
        Error.Conflict("Projects.ProjectAlreadyExists",
            "Project already exists.");
    
    public static readonly Error SystemProjectCannotBeDeleted =
        Error.Failure(
            "Projects.SystemProjectCannotBeDeleted",
            "System project cannot be deleted.");

    public static readonly Error SystemProjectCannotBeUpdated =
        Error.Failure(
            "Projects.SystemProjectCannotBeUpdated",
            "System project cannot be updated.");
}