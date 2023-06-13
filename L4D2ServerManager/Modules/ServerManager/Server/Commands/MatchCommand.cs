using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Modules.ServerManager.Server.Commands;

public class MatchCommand : RunScriptCommand
{
    public MatchCommand(int port, string matchName)
    {
        Script.Add($@"sudo screen -S {port} -X stuff 'sm_forcematch {matchName}\n'");
    }
}