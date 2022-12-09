using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;

namespace L4D2ServerManager.Contexts.AzureBlobAccount;

public class AzureBlobAccountContext : IAzureBlobAccountContext
{
    private readonly IConfiguration _configuration;

    private readonly Dictionary<string, BlobContainerClient> _containers = new();
    private BlobServiceClient? _blobServiceClient;

    public AzureBlobAccountContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string ConnectionString => _configuration.GetValue<string>("AzureWebJobsStorage")!;
    private BlobServiceClient BlobServiceClient => _blobServiceClient ??= new BlobServiceClient(ConnectionString);

    public async Task<string> GenerateSasUrlAsync(string container, string blobName, BlobSasPermissions permissions, DateTimeOffset expiresOn)
    {
        await CreateContainerIfNotExistsAsync(container);

        var blobClient = new BlobBaseClient(ConnectionString, container, blobName);
        var uri = blobClient.GenerateSasUri(permissions, expiresOn);
        var publicUrl = uri.ToString();

        return publicUrl;
    }

    public AsyncPageable<BlobContainerItem> GetBlobContainersAsync(string prefix, CancellationToken cancellationToken)
    {
        return BlobServiceClient.GetBlobContainersAsync(BlobContainerTraits.None, prefix, cancellationToken);
    }

    public BlobContainerClient GetBlobContainerClient(string blobContainerName)
    {
        return BlobServiceClient.GetBlobContainerClient(blobContainerName);
    }

    private async Task CreateContainerIfNotExistsAsync(string container)
    {
        if (_containers.ContainsKey(container)) return;

        var blobContainerClient = BlobServiceClient.GetBlobContainerClient(container);
        await blobContainerClient.CreateIfNotExistsAsync();

        _containers.Add(container, blobContainerClient);
    }
}