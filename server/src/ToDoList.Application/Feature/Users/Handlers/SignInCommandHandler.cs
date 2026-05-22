using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Common.Results;
using ToDoList.Application.Feature.Users.Commands;
using ToDoList.Application.Feature.Users.Models;
using ToDoList.Domain.Entities;

namespace ToDoList.Application.Feature.Users.Handlers;

public sealed class SignInCommandHandler(
    UserManager<ApplicationUser> userManager,
    IValidator<SignInCommand> validator,
    ITokenService tokenService)
    : IRequestHandler<SignInCommand, Result<AuthResponseModel>>
{
    public async Task<Result<AuthResponseModel>> Handle(
        SignInCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();

            return Result<AuthResponseModel>.Failure(
                Error.Validation(
                    firstError.ErrorCode,
                    firstError.ErrorMessage));
        }

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result<AuthResponseModel>.Failure(UserErrors.InvalidCredentials);
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            return Result<AuthResponseModel>.Failure(UserErrors.InvalidCredentials);
        }

        var token = tokenService.GenerateAccessToken(user);

        var response = new AuthResponseModel(
            UserId: user.Id,
            Email: user.Email!,
            AccessToken: token);

        return Result<AuthResponseModel>.Success(response);
    }
}