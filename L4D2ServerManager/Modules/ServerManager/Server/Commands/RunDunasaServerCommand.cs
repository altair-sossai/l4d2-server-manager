using L4D2ServerManager.Modules.ServerManager.Server.Enums;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Modules.ServerManager.Server.Commands;

public class RunDunasaServerCommand : RunScriptCommand
{
    public RunDunasaServerCommand(int port, Campaign campaign)
    {
        Script.Add($"sudo screen -d -m -S \"{port}\" /home/steam/dunasa/srcds_run -game left4dead2 -port {port} +sv_clockcorrection_msecs 25 -timeout 10 -tickrate 100 +map {campaign} -maxplayers 32 +servercfgfile server.cfg");
    }
}