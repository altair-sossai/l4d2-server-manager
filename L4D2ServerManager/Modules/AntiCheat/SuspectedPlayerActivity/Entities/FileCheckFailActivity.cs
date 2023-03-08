using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities.Infrastructure;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities;

public class FileCheckFailActivity : Activity
{
    public FileCheckFailActivity(long communityId)
        : base(communityId)
    {
    }

    public DateTime? FileCheckFail { get; set; } = DateTime.UtcNow;
}