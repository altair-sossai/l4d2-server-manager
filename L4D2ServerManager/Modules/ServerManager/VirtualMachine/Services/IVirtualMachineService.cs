namespace L4D2ServerManager.Modules.ServerManager.VirtualMachine.Services;

public interface IVirtualMachineService
{
    IVirtualMachine GetByName(string name);
}