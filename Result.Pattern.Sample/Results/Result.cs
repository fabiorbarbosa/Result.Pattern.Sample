using System.Net;
using Result.Pattern.Sample.Enums;

namespace Result.Pattern.Sample.Results;

/// <summary>
/// Represents a result of an operation that can be either successful or failed.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
/// <remarks>
/// This class is used to return a result of an operation that can be either successful or failed.
/// </remarks>
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
        if (success && errors?.Any())
            throw new ArgumentException("Result of success cannot have errors");

        if (!success && value is not null)
            throw new ArgumentException("Result of error cannot have value");

        IsSuccess = success;
        Type = type;
        Value = value;
        Errors = errors;
        ActionName = actionName;
        RouteValues = routeValues;
        StatusCode = statusCode;
    }

    /// <summary>
    /// Creates a successful result with the specified value and status code.
    /// </summary>
    /// <param name="value">The value returned on success.</param>
    /// <param name="statusCode">The status code to return.</param>
    /// <returns>A successful result with the specified value and status code.</returns>
    public static Result<T> Success(T value, HttpStatusCode? statusCode = HttpStatusCode.OK)
        => new(true, ResultType.Success, value, statusCode: statusCode);

    /// <summary>
    /// Creates a successful result with no content.
    /// </summary>
    /// <returns>A successful result with no content.</returns>
    public static Result<T> NoContent()
        => new(true, ResultType.NoContent, default);

    /// <summary>
    /// Creates a successful result with the specified value, action name, and route values.
    /// </summary>
    /// <param name="value">The value returned on success.</param>
    /// <param name="actionName">The name of the action to return.</param>
    /// <param name="routeValues">The route values to return.</param>
    /// <returns>A successful result with the specified value, action name, and route values.</returns>
    public static Result<T> Created(T value, string actionName, object routeValues)
    {
        if (string.IsNullOrWhiteSpace(actionName))
            throw new ArgumentNullException(nameof(actionName), "Necessário informar 'actionName'.");

        if (routeValues is null)
            throw new ArgumentNullException(nameof(routeValues), "Necessário informar 'routeValues'.");

        return new(true, ResultType.Created, value, actionName: actionName, routeValues: routeValues);
    }

    /// <summary>
    /// Creates a failed result with the specified type and errors.
    /// </summary>
    /// <param name="type">The type of the result.</param>
    /// <param name="errors">The errors to return.</param>
    /// <returns>A failed result with the specified type and errors.</returns>
    public static Result<T> Failure(ResultType type, IEnumerable<string> errors)
        => new(false, type, default, errors: errors);

    /// <summary>
    /// Creates a failed result with the specified type and error.
    /// </summary>
    /// <param name="type">The type of the result.</param>
    /// <param name="error">The error to return.</param>
    /// <returns>A failed result with the specified type and error.</returns>
    public static Result<T> Failure(ResultType type, string error)
        => new(false, type, default, errors: [error]);

    /// <summary>
    /// Creates a failed result with the type NotFound.
    /// </summary>
    /// <param name="message">The message to return.</param>
    /// <returns>A failed result with the type NotFound.</returns>
    public static Result<T> NotFound<T>(string message = "Recurso não encontrado")
        => Failure(ResultType.NotFound, message);

    /// <summary>
    /// Creates a failed result with the type Validation.
    /// </summary>
    /// <param name="errors">The errors to return.</param>
    /// <returns>A failed result with the type Validation.</returns>
    public static Result<T> ValidationError<T>(IEnumerable<string> errors)
        => Failure(ResultType.Validation, errors);

    /// <summary>
    /// Creates a failed result with the type Unauthorized.
    /// </summary>
    /// <param name="message">The message to return.</param>
    /// <returns>A failed result with the type Unauthorized.</returns>
    public static Result<T> Unauthorized<T>(string message = "Não autorizado")
        => Failure(ResultType.Unauthorized, message);

    /// <summary>
    /// Creates a failed result with the type Conflict.
    /// </summary>
    /// <param name="message">The message to return.</param>
    /// <returns>A failed result with the type Conflict.</returns>
    public static Result<T> Conflict<T>(string message = "Conflito detectado")
        => Failure(ResultType.Conflict, message);
}