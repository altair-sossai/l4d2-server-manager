using L4D2ServerManager.Contexts.AzureSubscription;

namespace L4D2ServerManager.Modules.ServerManager.VirtualMachine.Services;

public class VirtualMachineService : IVirtualMachineService
{
	private readonly IAzureSubscriptionContext _context;

	public VirtualMachineService(IAzureSubscriptionContext context)
	{
		_context = context;
	}

	public IVirtualMachine GetByName(string name)
	{
		return new VirtualMachine(_context, name);
	}
}