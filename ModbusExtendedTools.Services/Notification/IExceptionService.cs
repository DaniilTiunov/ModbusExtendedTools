using ModbusExtendedTools.Services.Abstractions;

namespace ModbusExtendedTools.Services.Notification;

public interface IExceptionService
{
    Result Try(Action action, Action<Exception>? onError = null);
    
    Result<T> Try<T>(Func<T> func, Action<Exception>? onError = null);
    
    Task<Result> TryAsync(Func<Task> action,
        Action<Exception>? onError = null);
    
    Task<Result<T>> TryAsync<T>(Func<Task<T>> func,
        Action<Exception>? onError = null);
}