using Result.Pattern.Sample.Models;

namespace Result.Pattern.Sample.Extensions;

public static class ResultExtension
{
    public static IResult ToOkResult<T>(this T data)
        => Results.Ok(ResultDataModel<T>.Success(data));

    public static IResult ToSingleResult<T>(this T data)
        => data is not null
            ? Results.Ok(ResultDataModel<T>.Success(data))
            : Results.NotFound(ResultDataModel<T>.Failure("Not Found"));

    public static IResult ToCreateResult<T>(this T data, string location)
        => Results.Created(location, ResultDataModel<T>.Success(data));

    public static IResult ToNotFoundResult<T>(this T? data)
        => Results.NotFound(ResultDataModel<T>.Failure("Not Found"));
}
