using System.Net.NetworkInformation;

namespace ModbusExtendedTools.Services.Adapters;

public class AdapterService
{
    public IEnumerable<NetworkInterface> GetAllNetworkInterfaces()
    {
         return NetworkInterface.GetAllNetworkInterfaces();
    }
}