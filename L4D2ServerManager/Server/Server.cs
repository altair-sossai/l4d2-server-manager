using L4D2ServerManager.Server.Commands;
using L4D2ServerManager.Server.Services;
using L4D2ServerManager.Users;
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
    public HashSet<string> Permissions { get; } = new();
    public string? StartedBy => VirtualMachine.StartedBy(Port);
    public DateTime? StartedAt => VirtualMachine.StartedAt(Port);

    public async Task RunAsync(User user)
    {
        if (IsRunning)
            return;

        var command = new RunServerCommand(Port);
        VirtualMachine.RunCommand(command);

        var values = new Dictionary<string, string>
        {
            {$"port-{Port}-started-by", user.Id},
            {$"port-{Port}-started-at", DateTime.UtcNow.ToString("O")}
        };

        await VirtualMachine.UpdateTagsAsync(values);

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