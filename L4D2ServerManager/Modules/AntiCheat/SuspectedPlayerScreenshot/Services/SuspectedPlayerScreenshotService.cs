using Azure.Storage.Blobs.Models;
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

    public async Task DeleteAllScreenshots(long communityId)
    {
        var container = $"screenshot-{communityId}";

        await _blobAccountContext.DeleteBlobContainerAsync(container);
    }

    public async Task DeleteOldScreenshotsAsync()
    {
        var limit = DateTime.UtcNow.AddDays(-7);

        await foreach (var containerItem in _blobAccountContext.GetBlobContainersAsync("screenshot-", CancellationToken.None))
        {
            var containerClient = _blobAccountContext.GetBlobContainerClient(containerItem.Name);

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                var mustBeDeleted = limit > blobItem.Properties.CreatedOn;
                if (!mustBeDeleted)
                    continue;

                await containerClient.DeleteBlobAsync(blobItem.Name, DeleteSnapshotsOption.IncludeSnapshots);
            }
        }
    }
}