using L4D2ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Server.Commands;

public class GetRunningServersCommand : RunScriptCommand
{
    public GetRunningServersCommand()
    {
        Script.Add("sudo screen -ls");
    }
}