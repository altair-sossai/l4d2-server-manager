using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using L4D2ServerManager.VirtualMachine.Enums;

namespace L4D2ServerManager.VirtualMachine.ValueObjects;

public class PortInfo
{
    public PortInfo(SecurityRuleData securityRuleData)
    {
        Status = securityRuleData.Access == SecurityRuleAccess.Allow ? PortStatus.Open : PortStatus.Close;
        Rules = securityRuleData.SourceAddressPrefix;
    }

    public PortStatus Status { get; }
    public string Rules { get; }
}