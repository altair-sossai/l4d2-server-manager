using L4D2ServerManager.Users;
using L4D2ServerManager.VirtualMachine;
using L4D2ServerManager.VirtualMachine.ValueObjects;

namespace L4D2ServerManager.Server;

public interface IServer
{
    IVirtualMachine VirtualMachine { get; }
    string IpAddress { get; }
    int Port { get; }
    bool IsRunning { get; }
    PortInfo PortInfo { get; }
    HashSet<string> Permissions { get; }
    string? StartedBy { get; }
    DateTime? StartedAt { get; }
    Task RunAsync(User user);
    void Stop();
    void KickAllPlayers();
    void GivePills();
    void FillSurvivorsHealth();
    Task OpenPortAsync(string ranges);
    Task ClosePortAsync();
}