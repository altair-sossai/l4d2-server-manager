using Azure.Storage.Sas;

namespace L4D2ServerManager.Contexts.AzureBlobAccount;

public interface IAzureBlobAccountContext
{
    Task<string> GenerateSasUrlAsync(string container, string blobName, BlobSasPermissions permissions, DateTimeOffset expiresOn);
}