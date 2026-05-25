using FluentAssertions;
using ToDoList.Application.Feature.Users.Commands;
using ToDoList.Application.Feature.Users.Validators;
using Xunit;

namespace ToDoList.Application.Tests.Validators;

public class UserValidatorsTests
{
    [Fact]
    public void SignUp_ShouldFail_WhenEmailInvalid()
    {
        var validator = new SignUpCommandValidator();
        var command = new SignUpCommand("invalid", "user", "password");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void SignUp_ShouldFail_WhenUserNameTooShort()
    {
        var validator = new SignUpCommandValidator();
        var command = new SignUpCommand("user@local", "ab", "password");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void SignUp_ShouldFail_WhenPasswordTooShort()
    {
        var validator = new SignUpCommandValidator();
        var command = new SignUpCommand("user@local", "user", "12345");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void SignUp_ShouldPass_WhenValid()
    {
        var validator = new SignUpCommandValidator();
        var command = new SignUpCommand("user@local", "user", "password");

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void SignIn_ShouldFail_WhenEmailEmpty()
    {
        var validator = new SignInCommandValidator();
        var command = new SignInCommand(string.Empty, "password");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void SignIn_ShouldFail_WhenPasswordEmpty()
    {
        var validator = new SignInCommandValidator();
        var command = new SignInCommand("user@local", string.Empty);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void SignIn_ShouldPass_WhenValid()
    {
        var validator = new SignInCommandValidator();
        var command = new SignInCommand("user@local", "password");

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
