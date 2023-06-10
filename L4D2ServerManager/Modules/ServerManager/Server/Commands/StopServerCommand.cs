using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Modules.ServerManager.Server.Commands;

public class StopServerCommand : RunScriptCommand
{
    public StopServerCommand(int port)
    {
        Script.Add($"sudo /home/steam/l4d2/bash/stop.sh {port}");
    }
}