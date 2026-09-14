using CommunityToolkit.Mvvm.ComponentModel;
using ModbusExtendedTools.Services.Theming;

namespace ModbusExtendedTools.Desktop.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IThemeService _themeService;

    [ObservableProperty] private bool _isDarkMode;

    public SettingsViewModel(IThemeService themeService)
    {
        _themeService = themeService;
        _isDarkMode = themeService.CurrentTheme == AppTheme.Dark;
    }

    partial void OnIsDarkModeChanged(bool value)
    {
        _themeService.Apply(value ? AppTheme.Dark : AppTheme.Light);
    }
}
