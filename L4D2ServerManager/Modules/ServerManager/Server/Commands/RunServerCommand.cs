using L4D2ServerManager.Modules.ServerManager.Server.Enums;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Modules.ServerManager.Server.Commands;

public class RunServerCommand : RunScriptCommand
{
    public RunServerCommand(int port, Campaign campaign, string serverCfgFile)
    {
        Script.Add($"sudo /home/steam/l4d2/bash/start.sh {port} {campaign} {serverCfgFile}");
    }
}