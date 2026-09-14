namespace ModbusExtendedTools.Services.Theming;

public interface IThemeService
{
    AppTheme CurrentTheme { get; }

    void Apply(AppTheme theme);
}