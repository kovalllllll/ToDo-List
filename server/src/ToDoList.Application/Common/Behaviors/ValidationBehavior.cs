using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Common.Results;

namespace ToDoList.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
        {
            logger.LogInformation("Validation passed for {RequestName}", typeof(TRequest).Name);

            return await next(cancellationToken);
        }

        var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));

        logger.LogWarning(
            "Validation failed for {RequestName}. Errors: {Errors}",
            typeof(TRequest).Name, errorMessage);

        var validationError = Error.Validation(
            "Validation.General",
            errorMessage);

        var failureResult = typeof(TResponse)
            .GetMethod("Failure", [typeof(Error)])?
            .Invoke(null, [validationError]);

        return (TResponse)failureResult!;
    }
}