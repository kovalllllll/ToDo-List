using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Users.Commands;
using ToDoList.Application.Feature.Users.Models;
using ToDoList.Domain.Entities;

namespace ToDoList.Application.Feature.Users.Handlers;

public sealed class SignUpCommandHandler(
    ILogger<SignUpCommandHandler> logger,
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService)
    : IRequestHandler<SignUpCommand, Result<AuthResponseModel>>
{
    public async Task<Result<AuthResponseModel>> Handle(
        SignUpCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            logger.LogWarning("User with email {Email} already exists", request.Email);

            return Result<AuthResponseModel>.Failure(UserErrors.EmailAlreadyExists);
        }

        var user = new ApplicationUser(request.Email, request.UserName);

        logger.LogInformation("Creating new user with email {Email}", request.Email);

        var createResult = await userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            var firstIdentityError = createResult.Errors.FirstOrDefault();

            logger.LogError(
                "User creation failed for email {Email}. Error: {ErrorCode} - {ErrorDescription}",
                request.Email,
                firstIdentityError?.Code,
                firstIdentityError?.Description);

            return Result<AuthResponseModel>.Failure(
                Error.Validation(
                    firstIdentityError?.Code ?? "Users.SignUpFailed",
                    firstIdentityError?.Description ?? "Registration failed."));
        }

        var token = tokenService.GenerateAccessToken(user);

        logger.LogInformation("User {UserId} created successfully", user.Id);

        var response = new AuthResponseModel(
            UserId: user.Id,
            Email: user.Email!,
            AccessToken: token);

        return Result<AuthResponseModel>.Success(response);
    }
}