using Microsoft.AspNetCore.Mvc;
using ToDoList.Application.Common.Errors;
using ToDoList.Application.Common.Results;

namespace ToDoList.API.Extensions;

public static class ResultExtensions
{
    public static IResult ToMinimalApiResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        var problemDetails = new ProblemDetails
        {
            Title = result.Error.Code,
            Detail = result.Error.Description,
            Status = GetStatusCode(result.Error.Type)
        };

        return result.Error.Type switch
        {
            ErrorType.Validation => TypedResults.BadRequest(problemDetails),
            ErrorType.NotFound => TypedResults.NotFound(problemDetails),
            ErrorType.Conflict => TypedResults.Conflict(problemDetails),
            ErrorType.Unauthorized => TypedResults.Unauthorized(),
            ErrorType.Forbidden => TypedResults.StatusCode(StatusCodes.Status403Forbidden),
            _ => TypedResults.Problem(
                title: result.Error.Code,
                detail: result.Error.Description,
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    public static IResult ToMinimalApiResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.NoContent();
        }

        var problemDetails = new ProblemDetails
        {
            Title = result.Error.Code,
            Detail = result.Error.Description,
            Status = GetStatusCode(result.Error.Type)
        };

        return result.Error.Type switch
        {
            ErrorType.Validation => TypedResults.BadRequest(problemDetails),
            ErrorType.NotFound => TypedResults.NotFound(problemDetails),
            ErrorType.Conflict => TypedResults.Conflict(problemDetails),
            ErrorType.Unauthorized => TypedResults.Unauthorized(),
            ErrorType.Forbidden => TypedResults.StatusCode(StatusCodes.Status403Forbidden),
            _ => TypedResults.Problem(
                title: result.Error.Code,
                detail: result.Error.Description,
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    private static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
}