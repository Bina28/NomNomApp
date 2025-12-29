namespace server.Features.Shared;

public record Result<T>
{
    public bool Success { get; }
    public T? Data { get; }
    public string? Error { get; }

    private Result(bool success, T? data, string? error)
    {
        Success = success;
        Data = data;
        Error = error;
    }

    public static Result<T> Ok(T data) => new(true, data, null);
    public static Result<T> Fail(string error) => new(false, default, error);
}