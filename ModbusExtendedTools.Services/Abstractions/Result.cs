namespace ModbusExtendedTools.Services.Abstractions;

public readonly record struct Result(bool Success, Exception? Error)
{
    public static Result Ok()
    {
        return new Result(true, null);
    }

    public static Result Fail(Exception ex)
    {
        return new Result(false, ex);
    }
}

public readonly record struct Result<T>(bool Success, T? Value, Exception? Error)
{
    public static Result<T> Ok(T value)
    {
        return new Result<T>(true, value, null);
    }

    public static Result<T> Fail(Exception ex)
    {
        return new Result<T>(false, default, ex);
    }
}