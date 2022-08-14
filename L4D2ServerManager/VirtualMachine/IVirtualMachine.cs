using L4D2ServerManager.VirtualMachine.Commands;
using L4D2ServerManager.VirtualMachine.Enums;
using L4D2ServerManager.VirtualMachine.Results;

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
}