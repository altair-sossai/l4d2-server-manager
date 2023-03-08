using Azure.ResourceManager.Resources;

namespace L4D2ServerManager.Contexts.AzureSubscription;

public interface IAzureSubscriptionContext
{
    SubscriptionResource SubscriptionResource { get; }
}