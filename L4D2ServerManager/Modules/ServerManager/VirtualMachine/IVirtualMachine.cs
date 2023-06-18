using L4D2ServerManager.Modules.Auth.Users;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Enums;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.ValueObjects;

namespace L4D2ServerManager.Modules.ServerManager.VirtualMachine;

public interface IVirtualMachine
{
    VirtualMachineStatus Status { get; }
    bool IsOn { get; }
    bool IsOff { get; }
    string IpAddress { get; }
    HashSet<string> Permissions { get; }
    string? PowerOnBy { get; }
    DateTime? PowerOnAt { get; }
    string? PowerOffBy { get; }
    DateTime? PowerOffAt { get; }
    int ShutdownAttempt { get; }
    Task PowerOnAsync(User user);
    Task PowerOffAsync(User user);
    Task RunCommandAsync(RunScriptCommand command);
    Task<PortInfo> GetPortInfoAsync(int port);
    Task OpenPortAsync(int port);
    Task ClosePortAsync(int port, IEnumerable<string> allowedIps);
    Task UpdateTagsAsync(IDictionary<string, string> values);
    string? StartedBy(int port);
    DateTime? StartedAt(int port);
    Task ClearShutdownAttemptAsync();
    Task IncrementShutdownAttemptAsync();
}