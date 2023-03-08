using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities.Infrastructure;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities;

public class ProcessActivity : Activity
{
    public ProcessActivity(long communityId)
        : base(communityId)
    {
    }

    public DateTime? Process { get; set; } = DateTime.UtcNow;
}