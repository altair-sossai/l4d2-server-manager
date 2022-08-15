using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Microsoft.Extensions.Configuration;

namespace L4D2ServerManager.Azure;

public class AzureContext : IAzureContext
{
    private static ArmClient? _armClient;
    private static SubscriptionResource? _subscriptionResource;
    private static TokenCredential? _tokenCredential;
    private readonly IConfiguration _configuration;

    public AzureContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string TenantId => _configuration.GetValue<string>(nameof(TenantId));
    private string ClientId => _configuration.GetValue<string>(nameof(ClientId));
    private string ClientSecret => _configuration.GetValue<string>(nameof(ClientSecret));
    private TokenCredential TokenCredential => _tokenCredential ??= new ClientSecretCredential(TenantId, ClientId, ClientSecret);
    private ArmClient ArmClient => _armClient ??= new ArmClient(TokenCredential);
    public SubscriptionResource SubscriptionResource => _subscriptionResource ??= ArmClient.GetDefaultSubscription();
}