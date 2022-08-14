using L4D2ServerManager.Server.Commands;
using L4D2ServerManager.Server.Results;
using L4D2ServerManager.VirtualMachine;

namespace L4D2ServerManager.Server;

public class Server : IServer
{
    public Server(IVirtualMachine virtualMachine, int port)
    {
        VirtualMachine = virtualMachine;
        Port = port;
    }

    public IVirtualMachine VirtualMachine { get; }
    public string IpAddress => VirtualMachine.IpAddress;
    public int Port { get; }

    public bool IsRunning
    {
        get
        {
            var command = new GetRunningServersCommand();
            var commandResult = VirtualMachine.RunCommandAsync(command).Result;
            var runningServersResult = new GetRunningServersResult(commandResult);

            return runningServersResult.Ports.Contains(Port);
        }
    }

    public async Task RunAsync()
    {
        if (IsRunning)
            return;

        var command = new RunServerCommand(Port);

        await VirtualMachine.RunCommandAsync(command);
    }

    public async Task StopAsync()
    {
        if (!IsRunning)
            return;

        var command = new StopServerCommand(Port);

        await VirtualMachine.RunCommandAsync(command);
    }
}