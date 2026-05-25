using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ToDoList.Domain.Entities;

namespace ToDoList.Application.Tests.TestHelpers;

internal static class UserManagerMock
{
    public static Mock<UserManager<ApplicationUser>> Create()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var options = Options.Create(new IdentityOptions());
        var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
        var userValidators = Array.Empty<IUserValidator<ApplicationUser>>();
        var passwordValidators = Array.Empty<IPasswordValidator<ApplicationUser>>();
        var keyNormalizer = new Mock<ILookupNormalizer>();
        var errors = new IdentityErrorDescriber();
        var services = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();

        return new Mock<UserManager<ApplicationUser>>(
            store.Object,
            options,
            passwordHasher.Object,
            userValidators,
            passwordValidators,
            keyNormalizer.Object,
            errors,
            services.Object,
            logger.Object);
    }
}
