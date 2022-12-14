using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities.Infrastructure;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities;

public class PingUnfocusedActivity : Activity
{
	public PingUnfocusedActivity(long communityId)
		: base(communityId)
	{
	}

	public DateTime? PingUnfocused { get; set; } = DateTime.UtcNow;
}