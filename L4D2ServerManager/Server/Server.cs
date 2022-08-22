using L4D2ServerManager.Server.Commands;
using L4D2ServerManager.Server.Services;
using L4D2ServerManager.VirtualMachine;
using L4D2ServerManager.VirtualMachine.ValueObjects;

namespace L4D2ServerManager.Server;

public class Server : IServer
{
    private readonly IServerService _serverService;

    public Server(IServerService serverService, IVirtualMachine virtualMachine, int port)
    {
        _serverService = serverService;
        VirtualMachine = virtualMachine;
        Port = port;
    }

    public IVirtualMachine VirtualMachine { get; }
    public string IpAddress => VirtualMachine.IpAddress;
    public int Port { get; }
    public bool IsRunning => _serverService.IsRunningAsync(IpAddress, Port).Result;
    public PortInfo PortInfo => VirtualMachine.GetPortInfoAsync(Port).Result;

    public void Run()
    {
        if (IsRunning)
            return;

        var command = new RunServerCommand(Port);

        VirtualMachine.RunCommand(command);

        WaitUntilItsRunning();
    }

    public void Stop()
    {
        if (!IsRunning)
            return;

        var command = new StopServerCommand(Port);

        VirtualMachine.RunCommand(command);
    }

    public void KickAllPlayers()
    {
        if (!IsRunning)
            return;

        var command = new KickAllPlayersCommand(Port);

        VirtualMachine.RunCommand(command);
    }

    public async Task OpenPortAsync(string ranges)
    {
        await VirtualMachine.OpenPortAsync(Port, ranges);
    }

    public async Task ClosePortAsync()
    {
        await VirtualMachine.ClosePortAsync(Port);
    }

    private void WaitUntilItsRunning()
    {
        for (var seconds = 5; !IsRunning && seconds <= 15; seconds++)
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
    }
}