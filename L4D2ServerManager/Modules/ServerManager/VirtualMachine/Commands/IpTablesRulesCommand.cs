using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;

namespace L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

public class IpTablesRulesCommand : RunScriptCommand
{
    public IpTablesRulesCommand(SecurityRuleCollection securityRules)
    {
        Script.Add("sudo iptables -F");
        Script.Add("sudo iptables -X");
        Script.Add("sudo iptables -P INPUT ACCEPT");
        Script.Add("sudo iptables -P OUTPUT ACCEPT");
        Script.Add("sudo iptables -P FORWARD ACCEPT");

        foreach (var securityRule in securityRules.Select(r => r.Data))
        {
            if (securityRule.Protocol != SecurityRuleProtocol.Udp || !int.TryParse(securityRule.DestinationPortRange, out var port))
                continue;

            Script.Add($"sudo iptables -A INPUT -p tcp --dport {port} -j DROP");
            Script.Add($"sudo iptables -A INPUT -p udp --dport {port} -m length --length 0:32 -j DROP");
            Script.Add($"sudo iptables -A INPUT -p udp --dport {port} -m length --length 2521:65535 -j DROP");

            if (securityRule.SourceAddressPrefix == "*")
                continue;

            foreach (var ip in securityRule.SourceAddressPrefixes)
                Script.Add($"sudo iptables -A INPUT -p udp --dport {port} -s {ip} -j ACCEPT");

            Script.Add($"sudo iptables -A INPUT -p udp --dport {port} -j DROP");
        }
    }
}