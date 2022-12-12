using L4D2ServerManager.Modules.ServerManager.Server.Enums;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Modules.ServerManager.Server.Commands;

public class ChangeMapCommand : RunScriptCommand
{
	public ChangeMapCommand(int port, Campaign campaign)
	{
		Script.Add($@"sudo screen -S {port} -X stuff 'map {campaign}\n'");
	}
}