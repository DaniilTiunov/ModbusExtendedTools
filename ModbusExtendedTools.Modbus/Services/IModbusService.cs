using ModbusExtendedTools.Modbus.Options;

namespace ModbusExtendedTools.Modbus.Services;

public interface IModbusService
{
    bool IsConnected { get; }
    Task ConnectAsync(ModbusOptions options, CancellationToken ct = default);
    Task<bool> CanConnectAsync(ModbusOptions options, CancellationToken ct = default);
    Task DisconnectAsync();
    Task<ushort[]> ReadHoldingRegistersAsync(ushort address, ushort count, CancellationToken ct = default);
    Task WriteRegisterAsync(ushort address, ushort value, CancellationToken ct = default);
    Task WriteRegistersAsync(ushort address, ushort[] values, CancellationToken ct = default);
}