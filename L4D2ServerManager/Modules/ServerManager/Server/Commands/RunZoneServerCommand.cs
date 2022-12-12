using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Modules.ServerManager.Server.Commands;

public class RunZoneServerCommand : RunScriptCommand
{
	public RunZoneServerCommand(int port)
	{
		Script.Add($"sudo screen -d -m -S \"{port}\" /home/steam/l4d2z/srcds_run -game left4dead2 -port {port} +sv_clockcorrection_msecs 25 -timeout 10 -tickrate 100 -maxplayers 32 +servercfgfile server.cfg");
	}
}