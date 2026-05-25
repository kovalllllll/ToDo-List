using FluentValidation;
using ToDoList.Application.Feature.TaskItems.Commands;

namespace ToDoList.Application.Feature.TaskItems.Validators;

public class UpdateTaskItemCommandValidator : AbstractValidator<UpdateTaskItemCommand>
{
    public UpdateTaskItemCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(50)
            .WithMessage("Title must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("Description must not exceed 200 characters.");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .When(x => x.Priority.HasValue)
            .WithMessage("Invalid priority value.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid status value.");

        RuleFor(x => x.DeadlineUtc)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.DeadlineUtc.HasValue)
            .WithMessage("Due date must be in the future.");
    }
}