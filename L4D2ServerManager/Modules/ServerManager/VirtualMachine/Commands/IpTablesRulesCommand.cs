namespace L4D2ServerManager.Modules.ServerManager.VirtualMachine.Commands;

public class IpTablesRulesCommand : RunScriptCommand
{
    public IpTablesRulesCommand(IEnumerable<Port.Port> ports)
        : this(ports.Select(p => p.PortNumber))
    {
    }

    private IpTablesRulesCommand(IEnumerable<int> ports)
    {
        Script.Add("sudo iptables -F");
        Script.Add("sudo iptables -X");
        Script.Add("sudo iptables -P INPUT ACCEPT");
        Script.Add("sudo iptables -P OUTPUT ACCEPT");
        Script.Add("sudo iptables -P FORWARD ACCEPT");

        foreach (var port in ports)
        {
            Script.Add($"sudo iptables -A INPUT -p tcp --destination-port {port} -j LOG --log-prefix \"SRCDS - RCON \" -m limit --limit 1/m --limit-burst 1");
            Script.Add($"sudo iptables -A INPUT -p tcp --destination-port {port} -j DROP");
            Script.Add($"sudo iptables -A INPUT -p udp --destination-port {port} -m length --length 0:32 -j LOG --log-prefix \"SRCDS - XSQUERY \" --log-ip-options -m limit --limit 1/m --limit-burst 1");
            Script.Add($"sudo iptables -A INPUT -p udp --destination-port {port} -m length --length 0:32 -j DROP");
            Script.Add($"sudo iptables -A INPUT -p udp --destination-port {port} -m length --length 2521:65535 -j LOG --log-prefix \"SRCDS - XLFRAG \" --log-ip-options -m limit --limit 1/m --limit-burst 1");
            Script.Add($"sudo iptables -A INPUT -p udp --destination-port {port} -m length --length 2521:65535 -j DROP");
        }

        Script.Add("sudo iptables -A INPUT -p udp -m state --state ESTABLISH -j ACCEPT");
        Script.Add("sudo iptables -A INPUT -p udp -m state --state NEW -m hashlimit --hashlimit-mode srcip,dstport --hashlimit-name StopDoS --hashlimit 1/s --hashlimit-burst 3 -j ACCEPT");
        Script.Add("sudo iptables -A INPUT -p udp -m state --state NEW -m hashlimit --hashlimit-mode srcip --hashlimit-name StopDoS --hashlimit 1/s --hashlimit-burst 3 -j ACCEPT");
        Script.Add("sudo iptables -A INPUT -p udp -j LOG --log-prefix \"UDP - SPAM \" --log-ip-options -m limit --limit 1/m --limit-burst 1");
        Script.Add("sudo iptables -A INPUT -p udp -j DROP");
    }
}