namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Extensions;

public static class SuspectedPlayerActivityExtensions
{
	public static Dictionary<long, SuspectedPlayerActivity> ToDictionary(this IEnumerable<SuspectedPlayerActivity> activities)
	{
		return activities.ToDictionary(k => k.CommunityId, v => v);
	}

	public static bool Expired(this SuspectedPlayerActivity activity)
	{
		return activity.PingExpired() || activity.ProcessExpired() || activity.ScreenshotExpired();
	}

	public static bool PingExpired(this SuspectedPlayerActivity activity)
	{
		if (activity.PingFocused == null && activity.PingUnfocused == null)
			return true;

		var focused = activity.PingFocused ?? DateTime.MinValue;
		var unfocused = activity.PingUnfocused ?? DateTime.MinValue;
		var ping = focused > unfocused ? focused : unfocused;
		var limit = DateTime.UtcNow.AddMinutes(-5);

		return limit > ping;
	}

	public static bool ProcessExpired(this SuspectedPlayerActivity activity)
	{
		if (activity.Process == null)
			return true;

		var limit = DateTime.UtcNow.AddMinutes(-5);

		return limit > activity.Process;
	}

	public static bool ScreenshotExpired(this SuspectedPlayerActivity activity)
	{
		if (activity.Screenshot == null)
			return true;

		var limit = DateTime.UtcNow.AddMinutes(-5);

		return limit > activity.Screenshot;
	}
}