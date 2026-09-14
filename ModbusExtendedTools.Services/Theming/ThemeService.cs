using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace ModbusExtendedTools.Services.Theming;

public sealed class ThemeService : IThemeService
{
    private const string LightPalette =
        "/ModbusExtendedTools.Desktop;component/Resources/Themes/LightColors.xaml";

    private const string DarkPalette =
        "/ModbusExtendedTools.Desktop;component/Resources/Themes/DarkColors.xaml";

    public AppTheme CurrentTheme { get; private set; } = AppTheme.Dark;

    public void Apply(AppTheme theme)
    {
        CurrentTheme = theme;

        switch (theme)
        {
            case AppTheme.System:
                ApplicationThemeManager.ApplySystemTheme();
                ApplicationThemeManager.Apply(ApplicationThemeManager.GetAppTheme(), WindowBackdropType.None);
                break;
            case AppTheme.Light:
                ApplicationThemeManager.Apply(ApplicationTheme.Light, WindowBackdropType.None);
                break;
            case AppTheme.Dark:
                ApplicationThemeManager.Apply(ApplicationTheme.Dark, WindowBackdropType.None);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(theme), theme, null);
        }

        var appliedTheme = ApplicationThemeManager.GetAppTheme();
        ReplaceApplicationPalette(
            appliedTheme == ApplicationTheme.Dark ? DarkPalette : LightPalette);

        if (Application.Current.TryFindResource("AccentColor") is Color accentColor)
        {
            ApplicationAccentColorManager.Apply(accentColor, appliedTheme);
        }
    }

    private static void ReplaceApplicationPalette(string palettePath)
    {
        var dictionaries = Application.Current.Resources.MergedDictionaries;
        var currentPalette = dictionaries.FirstOrDefault(dictionary =>
            dictionary.Source?.OriginalString.Contains(
                "Resources/Themes/",
                StringComparison.OrdinalIgnoreCase) == true);

        var replacement = new ResourceDictionary
        {
            Source = new Uri(palettePath, UriKind.Relative)
        };

        if (currentPalette is null)
        {
            dictionaries.Add(replacement);
            return;
        }

        dictionaries[dictionaries.IndexOf(currentPalette)] = replacement;
    }
}
