using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ModbusExtendedTools.Desktop.Views.Windows;
using ModbusExtendedTools.Services.Theming;
using Wpf.Ui.Appearance;
using Wpf.Ui.Markup;

namespace ModbusExtendedTools.Desktop.Bootstrap;

public sealed class App : Application
{
    private readonly IServiceProvider _services;
    private readonly IThemeService _themeService;

    public App(IServiceProvider services, IThemeService themeService)
    {
        _services = services;
        _themeService = themeService;

        Resources.MergedDictionaries.Add(new ThemesDictionary { Theme = ApplicationTheme.Dark });
        Resources.MergedDictionaries.Add(new ControlsDictionary());
        Resources.MergedDictionaries.Add(CreateDictionary(
            "Resources/Themes/DarkColors.xaml"));
        Resources.MergedDictionaries.Add(CreateDictionary(
            "Resources/Styles/Controls.xaml"));
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _themeService.Apply(AppTheme.Dark);

        MainWindow = _services.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    private static ResourceDictionary CreateDictionary(string source)
    {
        return new ResourceDictionary
        {
            Source = new Uri(source, UriKind.Relative)
        };
    }
}