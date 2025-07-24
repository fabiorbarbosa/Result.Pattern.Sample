using System.Net;
using Result.Pattern.Sample.Enums;

namespace Result.Pattern.Sample.Results;

public class Result<T>
{
    public bool IsSuccess { get; }
    public ResultType Type { get; }
    public T? Value { get; }
    public IEnumerable<string>? Errors { get; }
    public string? ActionName { get; }
    public object? RouteValues { get; }
    public HttpStatusCode? StatusCode { get; }

    protected Result(
        bool success,
        ResultType type,
        T? value,
        IEnumerable<string>? errors = default,
        string? actionName = default,
        object? routeValues = default,
        HttpStatusCode? statusCode = default)
    {
        IsSuccess = success;
        Type = type;
        Value = value;
        Errors = errors;
        ActionName = actionName;
        RouteValues = routeValues;
        StatusCode = statusCode;
    }

    public static Result<T> Success(T value, HttpStatusCode? statusCode = HttpStatusCode.OK)
        => new(true, ResultType.Success, value, statusCode: statusCode);

    public static Result<T> NoContent()
        => new(true, ResultType.NoContent, default);

    public static Result<T> Created(T value, string actionName, object routeValues)
    {
        if (string.IsNullOrWhiteSpace(actionName))
            throw new ArgumentNullException(nameof(actionName), "Necessário informar 'actionName'.");

        if (routeValues is null)
            throw new ArgumentNullException(nameof(routeValues), "Necessário informar 'routeValues'.");

        return new(true, ResultType.Created, value, actionName: actionName, routeValues: routeValues);
    }

    public static Result<T> Failure(ResultType type, IEnumerable<string> errors)
        => new(false, type, default, errors: errors);

    public static Result<T> Failure(ResultType type, string error)
        => new(false, type, default, errors: [error]);
}