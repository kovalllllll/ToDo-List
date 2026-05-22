namespace ToDoList.Application.Common.Errors;

public static class UserErrors
{
    public static readonly Error EmailAlreadyExists =
        Error.Conflict(
            "Users.EmailAlreadyExists",
            "A user with this email already exists.");

    public static readonly Error InvalidCredentials =
        Error.Unauthorized(
            "Users.InvalidCredentials",
            "Invalid email or password.");

    public static readonly Error UserNotFound =
        Error.NotFound(
            "Users.NotFound",
            "User was not found.");
}