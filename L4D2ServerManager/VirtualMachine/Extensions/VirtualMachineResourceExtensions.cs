using Azure.ResourceManager.Compute;

namespace L4D2ServerManager.VirtualMachine.Extensions;

public static class VirtualMachineResourceExtensions
{
    public static async Task UpdateTagValue(this VirtualMachineResource virtualMachine, string key, string value)
    {
        var tags = virtualMachine.Data.Tags;

        if (tags.ContainsKey(key))
            tags[key] = value;
        else
            tags.Add(key, value);

        await virtualMachine.SetTagsAsync(tags);
    }
}