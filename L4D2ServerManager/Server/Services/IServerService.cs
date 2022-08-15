using L4D2ServerManager.Server.Info.ValueObjects;
using L4D2ServerManager.VirtualMachine;

namespace L4D2ServerManager.Server.Services;

public interface IServerService
{
    IServer GetByPort(IVirtualMachine virtualMachine, int port);
    Task<bool> IsRunningAsync(string ip, int port);
    Task<ServerInfo?> GetServerInfoAsync(string ip, int port);
}