using L4D2ServerManager.Contexts.Steam.Structures;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Results;

public class CheckAntiCheatUsageResult
{
	public List<SuspectedPlayerResult> AreNotUsing { get; } = new();

	public void Add(SteamIdentifiers steamIdentifiers)
	{
		var result = new SuspectedPlayerResult(steamIdentifiers);

		AreNotUsing.Add(result);
	}
}