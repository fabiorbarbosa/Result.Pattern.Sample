using System.Net;
using Microsoft.AspNetCore.Mvc;
using Result.Pattern.Sample.Enums;
using Result.Pattern.Sample.Results;

namespace Result.Pattern.Sample.Extensions;

public static class ResultExtension
{
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
                => new ObjectResult(null) { StatusCode = 204 },

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
}
