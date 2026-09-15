namespace ModbusExtendedTools.Modbus.Options;

public class ModbusOptions
{
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 502;
    public byte UnitId { get; set; } = 1;
}