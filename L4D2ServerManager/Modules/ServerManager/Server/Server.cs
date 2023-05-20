using L4D2ServerManager.Modules.Auth.Users;
using L4D2ServerManager.Modules.ServerManager.Server.Commands;
using L4D2ServerManager.Modules.ServerManager.Server.Enums;
using L4D2ServerManager.Modules.ServerManager.Server.Services;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.ValueObjects;

namespace L4D2ServerManager.Modules.ServerManager.Server;

public class Server : IServer
{
    private static readonly HashSet<int> ServersInStartup = new();

    private readonly IServerService _serverService;
    private readonly IVirtualMachine _virtualMachine;

    public Server(IServerService serverService, IVirtualMachine virtualMachine, int port)
    {
        _serverService = serverService;
        _virtualMachine = virtualMachine;
        Port = port;
    }

    public string IpAddress => _virtualMachine.IpAddress;
    public int Port { get; }
    public bool IsRunning => _serverService.IsRunningAsync(IpAddress, Port).Result;
    public PortInfo PortInfo => _virtualMachine.GetPortInfoAsync(Port).Result;
    public HashSet<string> Permissions { get; } = new();
    public string? StartedBy => _virtualMachine.StartedBy(Port);
    public DateTime? StartedAt => _virtualMachine.StartedAt(Port);

    public async Task RunAsync(User user, Campaign campaign)
    {
        try
        {
            if (ServersInStartup.Contains(Port))
                return;

            ServersInStartup.Add(Port);

            var command = new RunServerCommand(Port, campaign);

            await RunAsync(user, command);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
        finally
        {
            ServersInStartup.Remove(Port);
        }
    }

    public void Stop()
    {
        if (!IsRunning)
            return;

        var command = new StopServerCommand(Port);

        _virtualMachine.RunCommand(command);
    }

    public void KickAllPlayers()
    {
        if (!IsRunning)
            return;

        var command = new KickAllPlayersCommand(Port);

        _virtualMachine.RunCommand(command);
    }

    public async Task OpenPortAsync()
    {
        await _virtualMachine.OpenPortAsync(Port);
    }

    public async Task ClosePortAsync(IEnumerable<string> allowedIps)
    {
        await _virtualMachine.ClosePortAsync(Port, allowedIps);
    }

    private async Task RunAsync(User user, RunScriptCommand command)
    {
        if (IsRunning)
            return;

        _virtualMachine.RunCommand(command);

        var values = new Dictionary<string, string>
        {
            { $"port-{Port}-started-by", user.Id },
            { $"port-{Port}-started-at", DateTime.UtcNow.ToString("u") }
        };

        await _virtualMachine.UpdateTagsAsync(values);

        WaitUntilItsRunning();
    }

    private void WaitUntilItsRunning()
    {
        for (var attempt = 0; !IsRunning && attempt < 15; attempt++)
            Thread.Sleep(TimeSpan.FromSeconds(5));
    }
}