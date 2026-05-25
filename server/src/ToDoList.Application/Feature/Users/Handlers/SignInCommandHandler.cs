using FluentValidation;
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

public sealed class SignInCommandHandler(
    ILogger<SignInCommandHandler> logger,
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService)
    : IRequestHandler<SignInCommand, Result<AuthResponseModel>>
{
    public async Task<Result<AuthResponseModel>> Handle(
        SignInCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            logger.LogWarning("Sign-in failed. User not found for {Email}", request.Email);

            return Result<AuthResponseModel>.Failure(UserErrors.InvalidCredentials);
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            logger.LogWarning(
                "Sign-in failed. Invalid password for user {UserId}",
                user.Id);

            return Result<AuthResponseModel>.Failure(UserErrors.InvalidCredentials);
        }

        var token = tokenService.GenerateAccessToken(user);

        logger.LogInformation(
            "Sign-in succeeded for user {UserId} with email {Email}",
            user.Id,
            user.Email);

        var response = new AuthResponseModel(
            UserId: user.Id,
            Email: user.Email!,
            AccessToken: token);

        return Result<AuthResponseModel>.Success(response);
    }
}