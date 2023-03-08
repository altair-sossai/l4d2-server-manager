using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities.Infrastructure;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities;

public class ScreenshotActivity : Activity
{
    public ScreenshotActivity(long communityId)
        : base(communityId)
    {
    }

    public DateTime? Screenshot { get; set; } = DateTime.UtcNow;
}