using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using CommunityToolkit.Mvvm.ComponentModel;
using ModbusExtendedTools.Extensions.Collections;
using ModbusExtendedTools.Services.Adapters;

namespace ModbusExtendedTools.Desktop.ViewModels;

public partial class NetworkViewModel : ObservableObject
{
    private readonly AdapterService _adapterService;
    
    [ObservableProperty] private ObservableCollection<NetworkInterface> _networkInterfaces = new();
    [ObservableProperty] private NetworkInterface? _selectedNetworkInterface;

    [ObservableProperty] private string _ipAddress;

    public NetworkViewModel(AdapterService adapterService)
    {
        _adapterService = adapterService;

        GetNetworkInterfaces();
    }

    private void GetNetworkInterfaces()
    {
        NetworkInterfaces = _adapterService.GetAllNetworkInterfaces().ToObservable();
    }

    partial void OnSelectedNetworkInterfaceChanged(NetworkInterface? value)
    {

    }
}
