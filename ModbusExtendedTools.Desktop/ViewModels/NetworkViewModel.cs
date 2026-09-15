using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModbusExtendedTools.Extensions.Collections;
using ModbusExtendedTools.Services.Adapters;
using ModbusExtendedTools.Services.Notification;

namespace ModbusExtendedTools.Desktop.ViewModels;

public partial class NetworkViewModel : ObservableObject
{
    private readonly AdapterService _adapterService;
    private readonly INotificationService _notificationService;

    [ObservableProperty] private string? _ipAddress;

    [ObservableProperty] private ObservableCollection<NetworkInterface> _networkInterfaces = [];
    [ObservableProperty] private NetworkInterface? _selectedNetworkInterface;
    [ObservableProperty] private string _subnetMask = "255.255.255.0";

    public NetworkViewModel(
        AdapterService adapterService,
        INotificationService notificationService)
    {
        _adapterService = adapterService;
        _notificationService = notificationService;

        GetNetworkInterfaces();
    }

    private void GetNetworkInterfaces()
    {
        NetworkInterfaces = _adapterService.GetAllNetworkInterfaces().ToObservable();
    }

    partial void OnSelectedNetworkInterfaceChanged(NetworkInterface? value)
    {
        if (value is null)
        {
            IpAddress = string.Empty;
            SubnetMask = "255.255.255.0";
        }

        var ipv4 = value?.GetIPProperties()
            .UnicastAddresses
            .FirstOrDefault(x =>
                x.Address.AddressFamily ==
                AddressFamily.InterNetwork);

        IpAddress = ipv4?.Address.ToString() ?? string.Empty;

        SubnetMask = ipv4?.IPv4Mask?.ToString()
                     ?? "255.255.255.0";
    }

    [RelayCommand]
    private void SetIpAddressAsync()
    {
        if (SelectedNetworkInterface is null) _notificationService.ShowInfo("Пожалуйста, выберите сетевой адаптер");

        if (!IsValidIpv4(IpAddress)) _notificationService.ShowInfo("Введена некорректная маска");
    }

    private static bool IsValidIpv4(string ipInput)
    {
        return IPAddress.TryParse(ipInput, out var address) &&
               address.AddressFamily ==
               AddressFamily.InterNetwork;
    }
}