using Azure.ResourceManager.Network;
using L4D2ServerManager.Infrastructure.Helpers;

namespace L4D2ServerManager.Modules.ServerManager.VirtualMachine.Extensions;

public static class SecurityRuleDataExtension
{
    public static IEnumerable<string> Ips(this SecurityRuleData securityRuleData)
    {
        if (securityRuleData.SourceAddressPrefix == "*")
            yield break;

        if (IpHelper.IsValidIpv4(securityRuleData.SourceAddressPrefix))
            yield return securityRuleData.SourceAddressPrefix;

        foreach (var validIp in securityRuleData.SourceAddressPrefixes.Where(IpHelper.IsValidIpv4))
            yield return validIp;
    }
}