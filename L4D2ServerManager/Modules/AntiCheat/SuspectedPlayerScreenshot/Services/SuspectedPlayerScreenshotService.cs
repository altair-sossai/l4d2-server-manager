using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using L4D2ServerManager.Contexts.AzureBlobAccount;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerScreenshot.Results;

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
		var blobContainerName = $"screenshot-{communityId}";
		var blobName = $"{long.MaxValue - DateTime.UtcNow.Ticks}.jpg";

		return _blobAccountContext.GenerateSasUrlAsync(blobContainerName, blobName, BlobSasPermissions.Create, DateTimeOffset.Now.AddDays(1));
	}

	public async Task DeleteAllScreenshotsAsync(long communityId)
	{
		var blobContainerName = $"screenshot-{communityId}";

		await _blobAccountContext.DeleteBlobContainerAsync(blobContainerName);
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

	public async Task<List<ScreenshotResult>> ScreenshotsAsync(long communityId, int skip, int take)
	{
		var blobContainerName = $"screenshot-{communityId}";
		var containerClient = _blobAccountContext.GetBlobContainerClient(blobContainerName);
		var screenshots = new List<ScreenshotResult>();

		if (!await containerClient.ExistsAsync())
			return screenshots;

		await foreach (var blobItem in containerClient.GetBlobsAsync())
		{
			if (skip-- > 0)
				continue;

			var url = await _blobAccountContext.GenerateSasUrlAsync(blobContainerName, blobItem.Name, BlobSasPermissions.Read, DateTimeOffset.Now.AddHours(8));
			var screenshot = new ScreenshotResult(url, blobItem);

			screenshots.Add(screenshot);

			if (screenshots.Count == take)
				break;
		}

		return screenshots;
	}
}