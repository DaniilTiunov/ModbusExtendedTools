using ModbusExtendedTools.Services.Abstractions;
using ModbusExtendedTools.Services.Notification;

namespace ModbusExtendedTools.Services.Services;

public class ExceptionService : IExceptionService
{
    private readonly INotificationService _notificationService;

    public ExceptionService(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Result Try(Action action, Action<Exception>? onError = null)
    {
        try
        {
            action();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            Handle(ex, onError);
            return Result.Fail(ex);
        }
    }

    public Result<T> Try<T>(Func<T> func, Action<Exception>? onError = null)
    {
        try
        {
            return Result<T>.Ok(func());
        }
        catch (Exception ex)
        {
            Handle(ex, onError);
            return Result<T>.Fail(ex);
        }
    }

    public async Task<Result> TryAsync(Func<Task> action, Action<Exception>? onError = null)
    {
        try
        {
            await action();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            Handle(ex, onError);
            return Result.Fail(ex);
        }
    }

    public async Task<Result<T>> TryAsync<T>(Func<Task<T>> func, Action<Exception>? onError = null)
    {
        try
        {
            return Result<T>.Ok(await func());
        }
        catch (Exception ex)
        {
            Handle(ex, onError);
            return Result<T>.Fail(ex);
        }
    }
    
    private void Handle(Exception ex, Action<Exception>? onError)
    {
        if (onError is not null)
        {
            onError(ex);
            return;
        }

        _notificationService.ShowError(ex.Message);
    }
}