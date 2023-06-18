using Azure.ResourceManager.Compute;

namespace L4D2ServerManager.Modules.ServerManager.VirtualMachine.Extensions;

public static class VirtualMachineResourceExtensions
{
    public static async Task UpdateTagsAsync(this VirtualMachineResource virtualMachine, IDictionary<string, string> values)
    {
        var tags = virtualMachine.Data.Tags;

        foreach (var (key, value) in values)
            tags[key] = value;

        await virtualMachine.SetTagsAsync(tags);
    }
}