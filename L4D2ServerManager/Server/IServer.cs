using L4D2ServerManager.VirtualMachine;

namespace L4D2ServerManager.Server;

public interface IServer
{
    IVirtualMachine VirtualMachine { get; }
    string IpAddress { get; }
    int Port { get; }
    bool IsRunning { get; }
    Task RunAsync();
    Task StopAsync();
}