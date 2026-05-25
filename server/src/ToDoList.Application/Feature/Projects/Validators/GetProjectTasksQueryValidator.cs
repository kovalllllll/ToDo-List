using FluentValidation;
using ToDoList.Application.Feature.Projects.Queries;

namespace ToDoList.Application.Feature.Projects.Validators;

public sealed class GetProjectTasksQueryValidator : AbstractValidator<GetProjectTasksQuery>
{
    public GetProjectTasksQueryValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Filter.Search)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Filter.Search));

        RuleFor(x => x.Filter.Statuses)
            .Must(x => x == null || x.Distinct().Count() == x.Count)
            .WithMessage("Statuses must not contain duplicates.");

        RuleFor(x => x.Filter.Priorities)
            .Must(x => x == null || x.Distinct().Count() == x.Count)
            .WithMessage("Priorities must not contain duplicates.");
    }
}