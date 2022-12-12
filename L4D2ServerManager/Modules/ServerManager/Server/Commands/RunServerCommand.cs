using L4D2ServerManager.Modules.ServerManager.Server.Enums;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Modules.ServerManager.Server.Commands;

public class RunServerCommand : RunScriptCommand
{
	public RunServerCommand(int port, Campaign campaign)
	{
		Script.Add($"sudo screen -d -m -S \"{port}\" /home/steam/l4d2/srcds_run -port {port} -tickrate 100 -secure +mp_gamemode versus +map {campaign}");
	}
}