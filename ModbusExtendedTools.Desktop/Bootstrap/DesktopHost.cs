using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModbusExtendedTools.Desktop.ViewModels;
using ModbusExtendedTools.Desktop.Views.Pages;
using ModbusExtendedTools.Desktop.Views.Windows;
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
                ConfigureViews(services);
                ConfigureViewModels(services);
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
    }

    private static void ConfigureViews(IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddTransient<SettingsPage>();
    }

    private static void ConfigureViewModels(IServiceCollection services)
    {
        services.AddTransient<MainWindowViewModel>();
    }
}