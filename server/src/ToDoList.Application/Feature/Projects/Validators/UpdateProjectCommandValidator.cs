using FluentValidation;
using ToDoList.Application.Feature.Projects.Commands;

namespace ToDoList.Application.Feature.Projects.Validators;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    private const string HexColorPattern = "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";

    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Project name is required.")
            .MaximumLength(100)
            .WithMessage("Project name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Project description must not exceed 500 characters.");

        RuleFor(x => x.Color)
            .Matches(HexColorPattern)
            .When(x => !string.IsNullOrWhiteSpace(x.Color))
            .WithMessage("Color must be a valid hex code (e.g. #FFFFFF).");
    }
}