using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using L4D2ServerManager.Infrastructure.Helpers;

namespace L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

public class IpTablesRulesCommand : RunScriptCommand
{
    public IpTablesRulesCommand(SecurityRuleCollection securityRules)
    {
        Script.Add("sudo iptables -F");
        Script.Add("sudo iptables -X");

        foreach (var securityRule in securityRules.Select(r => r.Data))
        {
            if (securityRule.Protocol != SecurityRuleProtocol.Udp || !int.TryParse(securityRule.DestinationPortRange, out var port))
                continue;

            Script.Add($"sudo iptables -A INPUT -p tcp --dport {port} -j DROP");
            Script.Add($"sudo iptables -A INPUT -p udp --dport {port} -m length --length 0:32 -j DROP");
            Script.Add($"sudo iptables -A INPUT -p udp --dport {port} -m length --length 2521:65535 -j DROP");

            if (securityRule.SourceAddressPrefix == "*")
                continue;

            var allowedIps = new HashSet<string>(securityRule.SourceAddressPrefixes)
            {
                securityRule.SourceAddressPrefix
            };

            foreach (var allowedIp in allowedIps.Where(IpHelper.IsValidIpv4))
                Script.Add($"sudo iptables -A INPUT -p udp --dport {port} -s {allowedIp} -j ACCEPT");

            Script.Add($"sudo iptables -A INPUT -p udp --dport {port} -j DROP");
        }
    }
}