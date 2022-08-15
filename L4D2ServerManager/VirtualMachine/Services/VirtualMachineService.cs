using L4D2ServerManager.Azure;

namespace L4D2ServerManager.VirtualMachine.Services;

public class VirtualMachineService : IVirtualMachineService
{
    private readonly IAzureContext _context;

    public VirtualMachineService(IAzureContext context)
    {
        _context = context;
    }

    public IVirtualMachine GetByName(string name)
    {
        return new VirtualMachine(_context, name);
    }
}