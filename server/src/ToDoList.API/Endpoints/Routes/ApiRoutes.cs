namespace ToDoList.API.Endpoints.Routes;

public abstract class ApiRoutes
{
    private const string Base = "/api";

    public static class Users
    {
        private const string BaseRoute = Base + "/users";
        public const string SignUp = BaseRoute + "/sign-up";
        public const string SignIn = BaseRoute + "/sign-in";
    }

    public static class Projects
    {
        private const string BaseRoute = Base + "/projects";
        public const string Create = BaseRoute;
        public const string GetAll = BaseRoute;
        public const string GetById = BaseRoute + "/{projectId}";
        public const string GetAllTasksByProjectId = BaseRoute + "/{projectId}/tasks";
        public const string Update = BaseRoute + "/{projectId}";
        public const string Delete = BaseRoute + "/{projectId}";
    }

    public static class Tasks
    {
        private const string BaseRoute = Base + "/tasks";
        public const string Create = BaseRoute;
        public const string GetById = BaseRoute + "/{taskId}";
        public const string Update = BaseRoute + "/{taskId}";
        public const string Delete = BaseRoute + "/{taskId}";
    }
}