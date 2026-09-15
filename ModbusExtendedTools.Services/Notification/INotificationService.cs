namespace ModbusExtendedTools.Services.Notification;

public interface INotificationService
{
    void ShowInfo(string message);
    void ShowError(string message);
    bool ShowConfirmation(string message);
}