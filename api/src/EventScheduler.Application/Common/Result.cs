namespace EventScheduler.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? Error { get; }
    public int StatusCode { get; }

    private Result(T data, int statusCode = 200)
    {
        IsSuccess = true;
        Data = data;
        StatusCode = statusCode;
    }

    private Result(string error, int statusCode)
    {
        IsSuccess = false;
        Error = error;
        StatusCode = statusCode;
    }

    public static Result<T> Success(T data, int statusCode = 200) => new(data, statusCode);
    public static Result<T> Failure(string error, int statusCode = 400) => new(error, statusCode);
}