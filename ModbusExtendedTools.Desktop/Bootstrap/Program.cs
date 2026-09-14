using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ModbusExtendedTools.Desktop.Bootstrap;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        using var host = DesktopHost.Build();
        host.Start();

        try
        {
            host.Services.GetRequiredService<App>().Run();
        }
        finally
        {
            host.StopAsync().GetAwaiter().GetResult();
        }
    }
}