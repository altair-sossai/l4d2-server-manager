using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Modules.ServerManager.Server.Commands;

public class RunServerCommand : RunScriptCommand
{
	public RunServerCommand(int port)
	{
		Script.Add($"sudo screen -d -m -S \"{port}\" /home/steam/l4d2/srcds_run -port {port} -tickrate 100 -secure +mp_gamemode versus +servercfgfile server.cfg");
	}
}