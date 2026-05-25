using FluentValidation;
using ToDoList.Application.Feature.TaskItems.Commands;

namespace ToDoList.Application.Feature.TaskItems.Validators;

public class DeleteTaskItemCommandValidator : AbstractValidator<DeleteTaskItemCommand>
{
    public DeleteTaskItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Task item Id must not be empty.");
    }
}