using FluentValidation;
using ToDoList.Application.Feature.Projects.Commands;

namespace ToDoList.Application.Feature.Projects.Validators;

public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project Id is required.");
    }
}