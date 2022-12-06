namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerScreenshot.Services;

public interface ISuspectedPlayerScreenshotService
{
    Task<string> GenerateUploadUrlAsync(long communityId);
}