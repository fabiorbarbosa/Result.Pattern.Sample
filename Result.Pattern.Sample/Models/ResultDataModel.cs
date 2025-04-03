
namespace Result.Pattern.Sample.Models;

public class ResultDataModel<T> : ResultModel
{
    public ResultDataModel(T? data, bool isSuccess = true, string message = "")
        : base(isSuccess, message)
        => Data = data;

    public T? Data { get; }

    public static ResultDataModel<T> Success(T data) => new(data);
    public static ResultDataModel<T> Failure(string message) => new(default, false, message);
}
