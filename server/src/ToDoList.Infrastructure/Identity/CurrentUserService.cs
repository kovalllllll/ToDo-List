using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ToDoList.Application.Abstractions;

namespace ToDoList.Infrastructure.Identity;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var userIdValue = httpContextAccessor
                .HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            
            return userIdValue == null
                ? throw new UnauthorizedAccessException("User is not authenticated or NameIdentifier claim missing")
                : Guid.Parse(userIdValue);
        }
    }

    public string? Email =>
        httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}