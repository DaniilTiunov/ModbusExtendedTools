using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModbusExtendedTools.Desktop.Models;
using ModbusExtendedTools.Extensions.Collections;
using ModbusExtendedTools.Modbus.Options;
using ModbusExtendedTools.Modbus.Services;
using ModbusExtendedTools.Services.Adapters;
using ModbusExtendedTools.Services.Notification;

namespace ModbusExtendedTools.Desktop.ViewModels;

public partial class NetworkViewModel : ObservableObject
{
    private readonly AdapterService _adapterService;
    private readonly IExceptionService _exceptionService;
    private readonly IModbusService _modbusService;
    private readonly INotificationService _notificationService;

    [ObservableProperty] private ObservableCollection<ModbusDevice> _modbusDevices = [];
    [ObservableProperty] private ObservableCollection<NetworkInterface> _networkInterfaces = [];
    [ObservableProperty] private NetworkInterface? _selectedNetworkInterface;
    [ObservableProperty] private string _ipAddress = string.Empty;
    [ObservableProperty] private string _ipParameter;
    [ObservableProperty] private string _endIpAddress = "192.168.0.254";
    [ObservableProperty] private string _startIpAddress = "192.168.0.1";
    [ObservableProperty] private string _subnetMask = "255.255.255.0";

    public NetworkViewModel(
        AdapterService adapterService,
        INotificationService notificationService,
        IExceptionService exceptionService,
        IModbusService modbusService)
    {
        _adapterService = adapterService;
        _notificationService = notificationService;
        _exceptionService = exceptionService;
        _modbusService = modbusService;

        ScanDevicesAsyncCommand = new AsyncRelayCommand(ScanDevicesAsync);

        GetNetworkInterfaces();
    }

    public IAsyncRelayCommand ScanDevicesAsyncCommand { get; }

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
            IpParameter = string.Empty;
            return;
        }

        var ipv4 = value?.GetIPProperties()
            .UnicastAddresses
            .FirstOrDefault(x =>
                x.Address.AddressFamily ==
                AddressFamily.InterNetwork);

        IpAddress = ipv4?.Address.ToString() ?? string.Empty;

        SubnetMask = ipv4?.IPv4Mask?.ToString()
                     ?? "255.255.255.0";

        IpParameter = _adapterService.IsDhcpEnabled(value.Id)
            ? "DHCP"
            : "Статика";
    }

    [RelayCommand]
    private void SetIpAddress()
    {
        if (SelectedNetworkInterface is null)
            _notificationService.ShowInfo("Пожалуйста, выберите сетевой адаптер");

        if (!IsValidIpv4(IpAddress))
            _notificationService.ShowInfo("Введена некорректная маска");

        var result = _exceptionService.Try(() =>
        {
            if (SelectedNetworkInterface != null)
                _adapterService.SetIpAddressAsync(
                    SelectedNetworkInterface,
                    IpAddress,
                    SubnetMask);
        }, ex => _notificationService.ShowError(ex.Message));

        if (result.Success)
        {
            OnSelectedNetworkInterfaceChanged(SelectedNetworkInterface);
            _notificationService.ShowInfo("Настройки успешно изменены");
        }
    }

    [RelayCommand]
    private void SetIpDynamic()
    {
        if (SelectedNetworkInterface is not null)
        {
            if (IpParameter == "DHCP")
            {
                _notificationService.ShowInfo("DHCP уже установлен");
                return;
            }

            var result =
                _exceptionService.Try(() => { _adapterService.SetDefaultIpAddress(SelectedNetworkInterface?.Id); },
                    ex => _notificationService.ShowError(ex.Message));

            if (result.Success)
            {
                _notificationService.ShowInfo("DHCP установлен успешно");
                OnSelectedNetworkInterfaceChanged(SelectedNetworkInterface);
            }
        }
    }

    private async Task ScanDevicesAsync()
    {
        if (!IPAddress.TryParse(StartIpAddress, out var startIp) ||
            startIp.AddressFamily != AddressFamily.InterNetwork ||
            !IPAddress.TryParse(EndIpAddress, out var endIp) ||
            endIp.AddressFamily != AddressFamily.InterNetwork)
        {
            _notificationService.ShowInfo("Укажите корректный диапазон IPv4-адресов");
            return;
        }

        var start = ToUInt32(startIp);
        var end = ToUInt32(endIp);

        if (start > end)
        {
            _notificationService.ShowInfo("Начальный IP должен быть меньше конечного");
            return;
        }

        const ulong maxAddressCount = 65_536;
        var addressCount = (ulong)end - start + 1;

        if (addressCount > maxAddressCount)
        {
            _notificationService.ShowInfo(
                $"Слишком большой диапазон. Максимум: {maxAddressCount} адресов");
            return;
        }

        ModbusDevices.Clear();
        var foundDevices = new ConcurrentBag<ModbusDevice>();

        await Parallel.ForEachAsync(
            EnumerateIpAddresses(start, end),
            new ParallelOptions { MaxDegreeOfParallelism = 32 },
            async (address, cancellationToken) =>
            {
                using var timeout =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeout.CancelAfter(TimeSpan.FromMilliseconds(500));

                var options = new ModbusOptions
                {
                    Host = address.ToString(),
                    Port = 502,
                    UnitId = 1
                };

                if (await _modbusService.CanConnectAsync(options, timeout.Token))
                {
                    foundDevices.Add(new ModbusDevice
                    {
                        IpAddress = options.Host,
                        PortNumber = options.Port
                    });
                }
            });

        ModbusDevices = new ObservableCollection<ModbusDevice>(
            foundDevices.OrderBy(x => ToUInt32(IPAddress.Parse(x.IpAddress))));

        _notificationService.ShowInfo(
            $"Поиск завершён. Найдено устройств: {ModbusDevices.Count}");
    }

    private static IEnumerable<IPAddress> EnumerateIpAddresses(uint start, uint end)
    {
        for (var address = (ulong)start; address <= end; address++)
            yield return FromUInt32((uint)address);
    }

    private static uint ToUInt32(IPAddress address)
    {
        var bytes = address.GetAddressBytes();

        return ((uint)bytes[0] << 24) |
               ((uint)bytes[1] << 16) |
               ((uint)bytes[2] << 8) |
               bytes[3];
    }

    private static IPAddress FromUInt32(uint address)
    {
        return new IPAddress(
        [
            (byte)(address >> 24),
            (byte)(address >> 16),
            (byte)(address >> 8),
            (byte)address
        ]);
    }

    private static bool IsValidIpv4(string ipInput)
    {
        return IPAddress.TryParse(ipInput, out var address) &&
               address.AddressFamily ==
               AddressFamily.InterNetwork;
    }
}
