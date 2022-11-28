using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Modules.ServerManager.Server.Commands;

public class StopServerCommand : RunScriptCommand
{
    public StopServerCommand(int port)
    {
        Script.Add($"sudo kill `screen -ls | grep {port} | awk -F . '{{print $1}}' | awk '{{print $1}}'`");
    }
}