using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Modules.ServerManager.Server.Commands;

public class KickAllPlayersCommand : RunScriptCommand
{
	public KickAllPlayersCommand(int port)
	{
		Script.Add($@"sudo screen -S {port} -X stuff 'sm_kick @all\n'");
	}
}