using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Modules.ServerManager.Server.Commands;

public class ResetMatchCommand : RunScriptCommand
{
    public ResetMatchCommand(int port, string matchName)
    {
        Script.Add($@"sudo screen -S {port} -X stuff 'sm_resetmatch\nsm_forcematch {matchName}\n'");
    }
}