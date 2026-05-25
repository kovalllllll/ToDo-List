using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Constants;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Feature.Users.Commands;
using ToDoList.Application.Feature.Users.Handlers;
using ToDoList.Application.Repositories;
using ToDoList.Application.Tests.TestHelpers;
using ToDoList.Domain.Entities;
using Xunit;

namespace ToDoList.Application.Tests.Handlers;

public class UserHandlersTests
{
    [Fact]
    public async Task SignUp_ShouldReturnConflict_WhenEmailExists()
    {
        var request = new SignUpCommand("user@local", "user", "password");
        var logger = new Mock<ILogger<SignUpCommandHandler>>();
        var userManager = UserManagerMock.Create();
        var tokenService = new Mock<ITokenService>();
        var projectRepository = new Mock<IProjectRepository>();
        var existingUser = new ApplicationUser(request.Email, request.UserName);

        userManager.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        var handler = new SignUpCommandHandler(
            logger.Object,
            userManager.Object,
            tokenService.Object,
            projectRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.EmailAlreadyExists);
    }

    [Fact]
    public async Task SignUp_ShouldReturnValidation_WhenCreateFails()
    {
        var request = new SignUpCommand("user@local", "user", "password");
        var logger = new Mock<ILogger<SignUpCommandHandler>>();
        var userManager = UserManagerMock.Create();
        var tokenService = new Mock<ITokenService>();
        var projectRepository = new Mock<IProjectRepository>();
        var identityError = new IdentityError { Code = "Users.SignUpFailed", Description = "Failed" };

        userManager.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);
        userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        var handler = new SignUpCommandHandler(
            logger.Object,
            userManager.Object,
            tokenService.Object,
            projectRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Validation(identityError.Code, identityError.Description));
    }

    [Fact]
    public async Task SignUp_ShouldReturnAuthResponse_WhenSuccessful()
    {
        var userId = Guid.NewGuid();
        var request = new SignUpCommand("user@local", "user", "password");
        var logger = new Mock<ILogger<SignUpCommandHandler>>();
        var userManager = UserManagerMock.Create();
        var tokenService = new Mock<ITokenService>();
        var projectRepository = new Mock<IProjectRepository>();

        userManager.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);
        userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .Callback<ApplicationUser, string>((user, _) => user.Id = userId)
            .ReturnsAsync(IdentityResult.Success);
        tokenService.Setup(x => x.GenerateAccessToken(It.IsAny<ApplicationUser>()))
            .Returns("token");

        var handler = new SignUpCommandHandler(
            logger.Object,
            userManager.Object,
            tokenService.Object,
            projectRepository.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("token");
        result.Value.Email.Should().Be(request.Email);
        result.Value.UserId.Should().Be(userId);

        projectRepository.Verify(x => x.CreateAsync(
                It.Is<Project>(project =>
                    project.UserId == userId &&
                    project.IsSystem &&
                    project.Name == ProjectDefaults.InboxName &&
                    project.Description == ProjectDefaults.InboxDescription &&
                    project.Color == ProjectDefaults.InboxColor),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SignIn_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        var request = new SignInCommand("user@local", "password");
        var logger = new Mock<ILogger<SignInCommandHandler>>();
        var userManager = UserManagerMock.Create();
        var tokenService = new Mock<ITokenService>();

        userManager.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);

        var handler = new SignInCommandHandler(
            logger.Object,
            userManager.Object,
            tokenService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task SignIn_ShouldReturnUnauthorized_WhenPasswordInvalid()
    {
        var request = new SignInCommand("user@local", "password");
        var logger = new Mock<ILogger<SignInCommandHandler>>();
        var userManager = UserManagerMock.Create();
        var tokenService = new Mock<ITokenService>();
        var user = new ApplicationUser(request.Email, "user") { Id = Guid.NewGuid() };

        userManager.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);
        userManager.Setup(x => x.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(false);

        var handler = new SignInCommandHandler(
            logger.Object,
            userManager.Object,
            tokenService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task SignIn_ShouldReturnAuthResponse_WhenSuccessful()
    {
        var request = new SignInCommand("user@local", "password");
        var logger = new Mock<ILogger<SignInCommandHandler>>();
        var userManager = UserManagerMock.Create();
        var tokenService = new Mock<ITokenService>();
        var user = new ApplicationUser(request.Email, "user") { Id = Guid.NewGuid() };

        userManager.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);
        userManager.Setup(x => x.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(true);
        tokenService.Setup(x => x.GenerateAccessToken(user))
            .Returns("token");

        var handler = new SignInCommandHandler(
            logger.Object,
            userManager.Object,
            tokenService.Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("token");
        result.Value.Email.Should().Be(request.Email);
        result.Value.UserId.Should().Be(user.Id);
    }
}
