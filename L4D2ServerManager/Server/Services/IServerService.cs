using L4D2ServerManager.VirtualMachine;

namespace L4D2ServerManager.Server.Services;

public interface IServerService
{
    IServer GetByPort(IVirtualMachine virtualMachine, int port);
    Task<ServerInfo.ValueObjects.ServerInfo> GetServerInfoAsync(IServer server);
}