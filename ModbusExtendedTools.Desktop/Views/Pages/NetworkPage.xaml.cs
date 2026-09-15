using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using ModbusExtendedTools.Desktop.Models;
using ModbusExtendedTools.Desktop.ViewModels;

namespace ModbusExtendedTools.Desktop.Views.Pages;

public partial class NetworkPage : Page
{
    public NetworkPage(NetworkViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }

    private void Device_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        var selected = ModbusDeviceList.SelectedItem as ModbusDevice;
        
        if (selected == null)
            return;
        
        Process.Start(new ProcessStartInfo
        {
            FileName = $"http://{selected?.IpAddress}:80",
            UseShellExecute = true
        });
    }
}