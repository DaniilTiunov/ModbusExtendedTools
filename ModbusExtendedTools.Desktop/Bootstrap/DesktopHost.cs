using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModbusExtendedTools.Desktop.Services;
using ModbusExtendedTools.Desktop.ViewModels;
using ModbusExtendedTools.Desktop.Views.Pages;
using ModbusExtendedTools.Desktop.Views.Windows;
using ModbusExtendedTools.Services.Adapters;
using ModbusExtendedTools.Services.Notification;
using ModbusExtendedTools.Services.Theming;

namespace ModbusExtendedTools.Desktop.Bootstrap;

public static class DesktopHost
{
    public static IHost Build()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                ConfigureApp(services);
                ConfigureAppServices(services);
                ConfigureViewModels(services);
                ConfigureViews(services);
            })
            .Build();
    }

    private static void ConfigureApp(IServiceCollection services)
    {
        services.AddSingleton<App>();
    }

    private static void ConfigureAppServices(IServiceCollection services)
    {
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<AdapterService>();
        services.AddScoped<INotificationService, NotificationService>();
    }

    private static void ConfigureViews(IServiceCollection services)
    {
        services.AddScoped<SettingsPage>();
        services.AddScoped<NetworkPage>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<NotifyWindow>();
    }

    private static void ConfigureViewModels(IServiceCollection services)
    {
        services.AddScoped<MainWindowViewModel>();
        services.AddScoped<SettingsViewModel>();
        services.AddScoped<NetworkViewModel>();
    }
}