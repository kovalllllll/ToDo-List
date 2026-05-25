using FluentValidation;
using ToDoList.Application.Feature.TaskItems.Commands;

namespace ToDoList.Application.Feature.TaskItems.Validators;

public class CreateTaskItemCommandValidator : AbstractValidator<CreateTaskItemCommand>
{
    public CreateTaskItemCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Title must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(200);

        RuleFor(x => x.ProjectId)
            .NotEqual(Guid.Empty)
            .When(x => x.ProjectId.HasValue)
            .WithMessage("Project Id must not be empty.");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .When(x => x.Priority.HasValue)
            .WithMessage("Invalid priority value.");

        RuleFor(x => x.DeadlineUtc)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.DeadlineUtc.HasValue)
            .WithMessage("Due date must be in the future.");
    }
}