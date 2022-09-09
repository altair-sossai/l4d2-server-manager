using L4D2ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Server.Commands;

public class GivePillsCommand : RunScriptCommand
{
    public GivePillsCommand(int port)
    {
        Script.Add($@"sudo screen -S {port} -X stuff 'sm_givepills\n'");
    }
}