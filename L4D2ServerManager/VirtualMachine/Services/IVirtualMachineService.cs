namespace L4D2ServerManager.VirtualMachine.Services;

public interface IVirtualMachineService
{
    IVirtualMachine GetByName(string name);
}