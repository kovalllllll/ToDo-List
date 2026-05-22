namespace ToDoList.Application.Feature.Users.Models;

public record AuthResponseModel(Guid UserId, string Email, string AccessToken);