namespace L4D2ServerManager.VirtualMachine.Services;

public interface IVirtualMachineService
{
    VirtualMachine GetByName(string name);
}