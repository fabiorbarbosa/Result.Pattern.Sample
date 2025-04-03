namespace Result.Pattern.Sample.Models;

public abstract class ResultModel
{
    protected ResultModel(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public bool IsSuccess { get; }
    public string Message { get; private set; }
}
