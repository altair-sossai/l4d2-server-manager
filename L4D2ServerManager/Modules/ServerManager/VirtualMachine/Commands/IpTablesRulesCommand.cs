using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using L4D2ServerManager.Infrastructure.Helpers;

namespace L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

public class IpTablesRulesCommand : RunScriptCommand
{
    public IpTablesRulesCommand(SecurityRuleCollection securityRules)
    {
        Iptables("-F");
        Iptables("-X");

        Iptables("-P INPUT ACCEPT");
        Iptables("-P OUTPUT ACCEPT");
        Iptables("-P FORWARD ACCEPT");

        foreach (var securityRule in securityRules.Select(r => r.Data))
        {
            if (securityRule.Protocol != SecurityRuleProtocol.Udp || !int.TryParse(securityRule.DestinationPortRange, out var port))
                continue;

            Iptables($"-A INPUT -p tcp --dport {port} -j DROP");
            Iptables($"-A INPUT -p udp --dport {port} -m length --length 0:32 -j DROP");
            Iptables($"-A INPUT -p udp --dport {port} -m length --length 2521:65535 -j DROP");

            if (securityRule.SourceAddressPrefix == "*")
                continue;

            var allowedIps = new HashSet<string>(securityRule.SourceAddressPrefixes)
            {
                securityRule.SourceAddressPrefix
            };

            foreach (var allowedIp in allowedIps.Where(IpHelper.IsValidIpv4))
                Iptables($"-A INPUT -p udp --dport {port} -s {allowedIp} -j ACCEPT");

            Iptables($"-A INPUT -p udp --dport {port} -j DROP");
        }

        Iptables("-A INPUT -p udp -m state --state ESTABLISH -j ACCEPT");
        Iptables("-A INPUT -p udp -m state --state NEW -m hashlimit --hashlimit-mode srcip,dstport --hashlimit-name StopDoS --hashlimit 1/s --hashlimit-burst 3 -j ACCEPT");
        Iptables("-A INPUT -p udp -m state --state NEW -m hashlimit --hashlimit-mode srcip --hashlimit-name StopDoS --hashlimit 1/s --hashlimit-burst 3 -j ACCEPT");
        Iptables("-A INPUT -p udp -j DROP");
    }

    private void Iptables(string rule)
    {
        Script.Add($"sudo iptables {rule}");
    }
}