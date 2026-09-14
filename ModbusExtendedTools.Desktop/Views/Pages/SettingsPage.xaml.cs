using System.Windows.Controls;
using ModbusExtendedTools.Desktop.ViewModels;

namespace ModbusExtendedTools.Desktop.Views.Pages;

public partial class SettingsPage : Page
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}