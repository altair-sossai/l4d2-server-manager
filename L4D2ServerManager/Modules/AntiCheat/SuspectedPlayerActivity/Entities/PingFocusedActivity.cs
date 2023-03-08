using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities.Infrastructure;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities;

public class PingFocusedActivity : Activity
{
    public PingFocusedActivity(long communityId)
        : base(communityId)
    {
    }

    public DateTime? PingFocused { get; set; } = DateTime.UtcNow;
}