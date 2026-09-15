using System.Windows;
using ModbusExtendedTools.Desktop.Enums;
using ModbusExtendedTools.Desktop.Views.Windows;
using ModbusExtendedTools.Services.Notification;

namespace ModbusExtendedTools.Desktop.Services;

public class NotificationService : INotificationService
{
    public void ShowInfo(string message)
    {
        var window = CreateWindow(message, NotificationType.Info, "Информация");
        window.ShowDialog();
    }

    public void ShowError(string message)
    {
        var window = CreateWindow(message, NotificationType.Error, "Ошибка");
        window.ShowDialog();
    }

    public bool ShowConfirmation(string message)
    {
        var window = CreateWindow(message, NotificationType.Confirmation, "Подтверждение");
        var result = window.ShowDialog();
        return result == true;
    }

    private NotifyWindow CreateWindow(string message, NotificationType type, string title)
    {
        return new NotifyWindow(message, type, title)
        {
            Owner = Application.Current.MainWindow
        };
    }
}