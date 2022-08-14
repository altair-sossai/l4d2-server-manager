using L4D2ServerManager.VirtualMachine.Commands;
using L4D2ServerManager.VirtualMachine.Enums;
using L4D2ServerManager.VirtualMachine.Results;
using L4D2ServerManager.VirtualMachine.ValueObjects;

namespace L4D2ServerManager.VirtualMachine;

public interface IVirtualMachine
{
    VirtualMachineStatus Status { get; }
    bool IsOn { get; }
    bool IsOff { get; }
    string IpAddress { get; }
    Task PowerOnAsync();
    Task PowerOffAsync();
    Task<RunScriptResult> RunCommandAsync(RunScriptCommand command);
    Task<PortInfo> GetPortInfoAsync(int port);
    Task OpenPortAsync(int port, string ranges);
    Task ClosePortAsync(int port);
}