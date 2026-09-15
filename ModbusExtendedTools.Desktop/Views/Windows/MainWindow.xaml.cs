using System.Windows;
using ModbusExtendedTools.Desktop.ViewModels;
using ModbusExtendedTools.Desktop.Views.Pages;
using Wpf.Ui.Controls;

namespace ModbusExtendedTools.Desktop.Views.Windows;

public partial class MainWindow : FluentWindow
{
    public MainWindow(
        MainWindowViewModel viewModel,
        IServiceProvider serviceProvider)
    {
        InitializeComponent();
        DataContext = viewModel;

        RootNavigationView.SetServiceProvider(serviceProvider);
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        RootNavigationView.Navigate(typeof(NetworkPage));
    }
}