using CommunityToolkit.Mvvm.ComponentModel;
using ModbusExtendedTools.Services.Theming;

namespace ModbusExtendedTools.Desktop.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IThemeService _themeService;

    [ObservableProperty] private AppTheme _theme;

    public SettingsViewModel(IThemeService themeService)
    {
        _themeService = themeService;
    }

    public void ApplyTheme(AppTheme theme)
    {
        _themeService.Apply(theme);
    }
}