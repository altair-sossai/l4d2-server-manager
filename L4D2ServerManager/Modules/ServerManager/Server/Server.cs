using L4D2ServerManager.Modules.Auth.Users;
using L4D2ServerManager.Modules.ServerManager.Server.Commands;
using L4D2ServerManager.Modules.ServerManager.Server.Enums;
using L4D2ServerManager.Modules.ServerManager.Server.Services;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Enums;

namespace L4D2ServerManager.Modules.ServerManager.Server;

public class Server : IServer
{
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
    public PortStatus PortStatus => _virtualMachine.GetPortStatusAsync(Port).Result;
    public HashSet<string> Permissions { get; } = new();
    public string? StartedBy => _virtualMachine.StartedBy(Port);
    public DateTime? StartedAt => _virtualMachine.StartedAt(Port);

    public async Task RunAsync(User user, Campaign campaign)
    {
        var command = new RunServerCommand(Port, campaign);

        await RunAsync(user, command);
    }

    public async Task StopAsync()
    {
        if (!IsRunning)
            return;

        var command = new StopServerCommand(Port);

        await _virtualMachine.RunCommandAsync(command);
    }

    public async Task OpenPortAsync()
    {
        await _virtualMachine.OpenPortAsync(Port);
    }

    public async Task ClosePortAsync(IEnumerable<string> allowedIps)
    {
        await _virtualMachine.ClosePortAsync(Port, allowedIps);
    }

    public async Task OpenSlotAsync()
    {
        const int minSlots = 1;
        const int maxSlots = 30;

        var serverInfo = await _serverService.GetServerInfoAsync(IpAddress, Port);
        var connectedPlayers = serverInfo?.Players ?? maxSlots;
        var slots = Math.Max(minSlots, Math.Min(maxSlots, connectedPlayers + 1));

        var command = new OpenSlotCommand(Port, slots);

        await _virtualMachine.RunCommandAsync(command);
    }

    private async Task RunAsync(User user, RunScriptCommand command)
    {
        if (IsRunning)
            return;

        await _virtualMachine.RunCommandAsync(command);

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
        for (var attempt = 1; !IsRunning && attempt <= 15; attempt++)
            Thread.Sleep(TimeSpan.FromSeconds(attempt));
    }
}