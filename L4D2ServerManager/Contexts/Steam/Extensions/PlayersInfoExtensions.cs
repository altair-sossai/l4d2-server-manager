using L4D2ServerManager.Contexts.Steam.ValueObjects;

namespace L4D2ServerManager.Contexts.Steam.Extensions;

public static class PlayersInfoExtensions
{
	public static PlayerInfo? FirstPlayerOrDefault(this PlayersInfo? playersInfo)
	{
		return playersInfo?.Players?.FirstOrDefault();
	}
}