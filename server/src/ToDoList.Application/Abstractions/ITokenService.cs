using ToDoList.Domain.Entities;

namespace ToDoList.Application.Abstractions;

public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user);
}