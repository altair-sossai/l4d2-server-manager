using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace L4D2ServerManager.Azure;

public interface IAzureContext
{
    ArmClient ArmClient { get; }
    SubscriptionResource SubscriptionResource { get; }
}