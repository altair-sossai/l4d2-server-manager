using L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

namespace L4D2ServerManager.Modules.ServerManager.Server.Commands;

public class OpenSlotCommand : RunScriptCommand
{
    public OpenSlotCommand(int port, int slots)
    {
        Script.Add($@"sudo screen -S {port} -X stuff 'sm_fslots {slots}\n'");
    }
}