using ModbusExtendedTools.Desktop.ViewModels;
using Wpf.Ui.Controls;

namespace ModbusExtendedTools.Desktop.Views.Windows;

public partial class MainWindow : FluentWindow
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}