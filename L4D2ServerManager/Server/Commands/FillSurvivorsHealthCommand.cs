using L4D2ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Server.Commands;

public class FillSurvivorsHealthCommand : RunScriptCommand
{
    public FillSurvivorsHealthCommand(int port)
    {
        Script.Add($@"sudo screen -S {port} -X stuff 'say !sethpplayer @survivors 100\n'");
    }
}