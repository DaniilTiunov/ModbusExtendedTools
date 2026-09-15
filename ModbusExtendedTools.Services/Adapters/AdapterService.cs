using System.Management;
using System.Net.NetworkInformation;

namespace ModbusExtendedTools.Services.Adapters;

public class AdapterService
{
    public IEnumerable<NetworkInterface> GetAllNetworkInterfaces()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(x =>
                x.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                x.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            .OrderBy(x => x.Name);
    }

    public void SetIpAddressAsync(
        NetworkInterface networkInterface,
        string ipAddress,
        string subnetMask)
    {
        using var configuration =
            FindConfiguration(networkInterface.Id);

        var result = configuration.InvokeMethod(
            "EnableStatic",
            [
                new[] { ipAddress },
                new[] { subnetMask }
            ]);

        var resultCode = Convert.ToUInt32(result);

        // 0 — выполнено;
        // 1 — выполнено, требуется перезагрузка.
        if (resultCode is not 0 and not 1)
            throw new InvalidOperationException(
                $"Не удалось изменить IP. Код WMI: {resultCode}");
    }

    private static ManagementObject FindConfiguration(string adapterId)
    {
        using var searcher = new ManagementObjectSearcher(
            "SELECT * FROM Win32_NetworkAdapterConfiguration");

        foreach (ManagementObject configuration in searcher.Get())
        {
            var settingId = configuration["SettingID"]?.ToString();

            if (string.Equals(
                    settingId,
                    adapterId,
                    StringComparison.OrdinalIgnoreCase))
                return configuration;

            configuration.Dispose();
        }

        throw new InvalidOperationException(
            "Конфигурация сетевого адаптера не найдена.");
    }
}