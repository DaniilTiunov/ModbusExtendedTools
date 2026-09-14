using System.Windows.Controls;
using ModbusExtendedTools.Desktop.ViewModels;

namespace ModbusExtendedTools.Desktop.Views.Pages;

public partial class NetworkPage : Page
{
    public NetworkPage(NetworkViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}