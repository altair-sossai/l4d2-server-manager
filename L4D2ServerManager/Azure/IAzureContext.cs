using Azure.ResourceManager.Resources;

namespace L4D2ServerManager.Azure;

public interface IAzureContext
{
    SubscriptionResource SubscriptionResource { get; }
}