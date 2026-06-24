namespace ChatApp.SignalRTester.Models;

public class ApiResult<T>
{
    public bool IsSuccess { get; init; }

    public T? Data { get; init; }

    public string? ErrorMessage { get; init; }

    public static ApiResult<T> Success(T data)
    {
        return new ApiResult<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static ApiResult<T> Failure(string message)
    {
        return new ApiResult<T>
        {
            IsSuccess = false,
            ErrorMessage = message
        };
    }
}