using System.Management;
using System.Net.NetworkInformation;

namespace ModbusExtendedTools.Services.Adapters;

public class AdapterService
{
    public IEnumerable<NetworkInterface> GetAllNetworkInterfaces()
    {
        var physicalAdapterIds = GetPhysicalAdapterIds();

        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(x =>
                Guid.TryParse(x.Id, out var adapterId) &&
                physicalAdapterIds.Contains(adapterId))
            .OrderBy(x => x.Name)
            .ToArray();
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

                
        if (resultCode == 0x80070005)
            throw new UnauthorizedAccessException(
                "Для изменения IP-адреса запустите приложение от имени администратора.");
        
        // 0 — выполнено;
        // 1 — выполнено, требуется перезагрузка.
        if (resultCode is not 0 and not 1)
            throw new InvalidOperationException(
                $"Не удалось изменить IP. Код WMI: {resultCode}");
    }

    public void SetDefaultIpAddress(string? adapterId)
    {
        using var configuration = FindConfiguration(adapterId);

        // IP-адрес и маска подсети будут получаться от DHCP-сервера.
        using var dhcpResult = configuration.InvokeMethod(
            "EnableDHCP",
            inParameters: null,
            options: null);

        ValidateWmiResult(dhcpResult, "включить DHCP");

        // null удаляет заданный вручную список DNS-серверов.
        using var dnsParameters =
            configuration.GetMethodParameters("SetDNSServerSearchOrder");
        dnsParameters["DNSServerSearchOrder"] = null;

        using var dnsResult = configuration.InvokeMethod(
            "SetDNSServerSearchOrder",
            dnsParameters,
            options: null);

        ValidateWmiResult(dnsResult, "включить автоматическое получение DNS");
    }
    
    public bool IsDhcpEnabled(string adapterId)
    {
        using var searcher = new ManagementObjectSearcher(
            $"SELECT DHCPEnabled FROM Win32_NetworkAdapterConfiguration WHERE SettingID = '{adapterId}'");

        foreach (ManagementObject obj in searcher.Get())
        {
            return obj["DHCPEnabled"] is bool b && b;
        }

        return false;
    }

    private static void ValidateWmiResult(
        ManagementBaseObject? result,
        string operation)
    {
        if (result?["ReturnValue"] is null)
            throw new InvalidOperationException(
                $"WMI не вернул результат операции: {operation}.");

        var resultCode = Convert.ToUInt32(result["ReturnValue"]);

        if (resultCode is 91 or 0x80070005)
            throw new UnauthorizedAccessException(
                "Для изменения параметров адаптера запустите приложение от имени администратора.");

        // 0 — выполнено; 1 — выполнено, требуется перезагрузка.
        if (resultCode is not 0 and not 1)
            throw new InvalidOperationException(
                $"Не удалось {operation}. Код WMI: {resultCode}.");
    }

    private static ManagementObject FindConfiguration(string? adapterId)
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
    
    private static HashSet<Guid> GetPhysicalAdapterIds()
    {
        var adapterIds = new HashSet<Guid>();

        using var searcher = new ManagementObjectSearcher(
            "SELECT GUID FROM Win32_NetworkAdapter " +
            "WHERE PhysicalAdapter = TRUE AND GUID IS NOT NULL");

        using var adapters = searcher.Get();

        foreach (ManagementObject adapter in adapters)
        {
            using (adapter)
            {
                if (Guid.TryParse(adapter["GUID"]?.ToString(), out var adapterId))
                    adapterIds.Add(adapterId);
            }
        }

        return adapterIds;
    }
}
