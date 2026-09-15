using System.Net.Sockets;
using ModbusExtendedTools.Modbus.Options;
using NModbusAsync;

namespace ModbusExtendedTools.Modbus.Services;

public class ModbusService : IModbusService
{
    private TcpClient? _tcp;
    private IModbusMaster? _master;
    private ModbusOptions? _options;
    
    public bool IsConnected => _tcp?.Connected == true;
    
    public async Task ConnectAsync(
        ModbusOptions options,
        CancellationToken ct = default)
    {
        if (IsConnected)
            return;
    
        _options = options;
    
        _tcp = new TcpClient();
        await _tcp.Client.ConnectAsync(_options.Host, _options.Port, ct);
    
        var factory = new ModbusFactory();
        _master = factory.CreateTcpMaster(_tcp);
    }

    public async Task<bool> CanConnectAsync(ModbusOptions options, CancellationToken ct = default)
    {
        using var tcp = new TcpClient();

        try
        {
            await tcp.Client.ConnectAsync(options.Host, options.Port, ct);
            return tcp.Connected;
        }
        catch (SocketException)
        {
            return false;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return false;
        }
    }

    public Task DisconnectAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<ushort[]> ReadHoldingRegistersAsync(
        ushort startAddress,
        ushort count,
        CancellationToken ct = default)
    {
        if (_master is null || _options is null)
            throw new InvalidOperationException("Modbus не подключён.");

        return await _master.ReadHoldingRegistersAsync(
            _options.UnitId,
            startAddress,       
            count,
            ct);            
    }

    public Task WriteRegisterAsync(ushort address, ushort value, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task WriteRegistersAsync(ushort address, ushort[] values, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
