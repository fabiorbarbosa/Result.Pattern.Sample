using System.Net;
using Microsoft.AspNetCore.Mvc;
using Result.Pattern.Sample.Enums;
using Result.Pattern.Sample.Results;

namespace Result.Pattern.Sample.Extensions;

/// <summary>
/// Extension methods for the Result class.
/// </summary>
public static class ResultExtension
{
    /// <summary>
    /// Converts a Result object to an ObjectResult.
    /// </summary>
    /// <typeparam name="T">The type of the value returned on success.</typeparam>
    /// <param name="result">The Result object to convert.</param>
    /// <returns>An ObjectResult based on the result type.</returns>
    public static ObjectResult ToObjectResult<T>(this Result<T> result)
        => result.Type switch
        {
            ResultType.Success when result.StatusCode is null
                => new OkObjectResult(result.Value),

            ResultType.Success when result.StatusCode is not null
                => new ObjectResult(result.Value) { StatusCode = (int)result.StatusCode },

            ResultType.Created when result.ActionName is not null && result.RouteValues is not null
                => new CreatedAtActionResult(result.ActionName, null, result.RouteValues, result.Value),

            ResultType.NoContent
                => new NoContentResult(),

            ResultType.Failure
                => new ObjectResult(new { errors = result.Errors }) { StatusCode = 500 },

            ResultType.NotFound
                => new NotFoundObjectResult(result.Errors),

            ResultType.Validation
                => new UnprocessableEntityObjectResult(result.Value),

            ResultType.Conflict
                => new ConflictObjectResult(result.Errors),

            ResultType.Unauthorized
                => new UnauthorizedObjectResult(result.Value),

            _ => new ObjectResult(result.Value) { StatusCode = (int?)(result.StatusCode ?? HttpStatusCode.OK) }
        };

    /// <summary>
    /// Executes an action if the result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value returned on success.</typeparam>
    /// <param name="result">The Result object to execute the action on.</param>
    /// <param name="action">The action to execute if the result is successful.</param>
    /// <param name="logger">The logger to use for logging errors.</param>
    /// <returns>The Result object.</returns>
    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action, ILogger? logger = default)
    {
        if (result.IsSuccess && result.Value is not null)
        {
            try
            {
                action(result.Value);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error executing success action.");
            }
        }

        return result;
    }

    /// <summary>
    /// Executes an action if the result is not successful.
    /// </summary>
    /// <typeparam name="T">The type of the value returned on success.</typeparam>
    /// <param name="result">The Result object to execute the action on.</param>
    /// <param name="action">The action to execute if the result is not successful.</param>
    /// <param name="logger">The logger to use for logging errors.</param>
    /// <returns>The Result object.</returns>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<T> action, ILogger? logger = default)
    {
        if (!result.IsSuccess && result.Errors is not null)
        {
            try
            {
                action(result.Value);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error executing error action.");
            }
        }

        return result;
    }
}
