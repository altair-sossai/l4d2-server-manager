using Azure.Storage.Sas;
using L4D2ServerManager.Contexts.AzureBlobAccount;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerScreenshot.Services;

public class SuspectedPlayerScreenshotService : ISuspectedPlayerScreenshotService
{
    private readonly IAzureBlobAccountContext _blobAccountContext;

    public SuspectedPlayerScreenshotService(IAzureBlobAccountContext blobAccountContext)
    {
        _blobAccountContext = blobAccountContext;
    }

    public Task<string> GenerateUploadUrlAsync(long communityId)
    {
        var container = $"screenshot-{communityId}";
        var blobName = $"{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}.jpg";

        return _blobAccountContext.GenerateSasUrlAsync(container, blobName, BlobSasPermissions.Create, DateTimeOffset.Now.AddDays(1));
    }
}