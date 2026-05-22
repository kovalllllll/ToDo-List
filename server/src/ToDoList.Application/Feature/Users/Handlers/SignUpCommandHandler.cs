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

public sealed class SignUpCommandHandler(
    UserManager<ApplicationUser> userManager,
    IValidator<SignUpCommand> validator,
    ITokenService tokenService)
    : IRequestHandler<SignUpCommand, Result<AuthResponseModel>>
{
    public async Task<Result<AuthResponseModel>> Handle(
        SignUpCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First();

            return Result<AuthResponseModel>.Failure(
                Error.Validation(firstError.ErrorCode, firstError.ErrorMessage));
        }

        var existingUser = await userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return Result<AuthResponseModel>.Failure(UserErrors.EmailAlreadyExists);
        }

        var user = new ApplicationUser(request.Email, request.UserName);

        var createResult = await userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            var firstIdentityError = createResult.Errors.FirstOrDefault();

            return Result<AuthResponseModel>.Failure(
                Error.Validation(
                    firstIdentityError?.Code ?? "Users.SignUpFailed",
                    firstIdentityError?.Description ?? "Registration failed."));
        }

        var token = tokenService.GenerateAccessToken(user);

        var response = new AuthResponseModel(
            UserId: user.Id,
            Email: user.Email!,
            AccessToken: token);

        return Result<AuthResponseModel>.Success(response);
    }
}