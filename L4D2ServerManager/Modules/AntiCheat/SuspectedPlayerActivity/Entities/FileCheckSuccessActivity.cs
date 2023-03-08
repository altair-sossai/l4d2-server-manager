using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities.Infrastructure;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities;

public class FileCheckSuccessActivity : Activity
{
    public FileCheckSuccessActivity(long communityId)
        : base(communityId)
    {
    }

    public DateTime? FileCheckSuccess { get; set; } = DateTime.UtcNow;
}