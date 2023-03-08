using Azure.Storage.Blobs.Models;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerScreenshot.Results;

public class ScreenshotResult
{
    public ScreenshotResult(string url, BlobItem blobItem)
    {
        Url = url;
        CreatedOn = blobItem.Properties.CreatedOn;
    }

    public string? Url { get; }
    public DateTimeOffset? CreatedOn { get; }
}