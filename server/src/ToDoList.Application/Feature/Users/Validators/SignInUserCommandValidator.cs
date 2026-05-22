using FluentValidation;
using ToDoList.Application.Feature.Users.Commands;

namespace ToDoList.Application.Feature.Users.Validators;

public class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}