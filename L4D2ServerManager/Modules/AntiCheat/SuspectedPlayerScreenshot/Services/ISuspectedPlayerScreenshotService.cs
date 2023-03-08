using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerScreenshot.Results;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerScreenshot.Services;

public interface ISuspectedPlayerScreenshotService
{
    Task<string> GenerateUploadUrlAsync(long communityId);
    Task DeleteAllScreenshotsAsync(long communityId);
    Task DeleteOldScreenshotsAsync();
    Task<List<ScreenshotResult>> ScreenshotsAsync(long communityId, int skip, int take);
}