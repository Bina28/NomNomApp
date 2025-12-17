namespace server.Features.Shared;

public static class Result
{
    public static ResultValue Success() => new(true);
    public static ResultValue Fail(string error) => new(false, error);

    public static ResultValue<T> Success<T>(T data) => new(true, data);
    public static ResultValue<T> Fail<T>(string error) => new ResultValue<T>(false, default, error);
}


public record ResultValue(bool Ok, string? Error = null);
public record ResultValue<T>(bool Ok, T? Data = default, string? Error = null);
