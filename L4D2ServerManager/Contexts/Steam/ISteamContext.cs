using L4D2ServerManager.Contexts.Steam.Services;

namespace L4D2ServerManager.Contexts.Steam;

public interface ISteamContext
{
	public string SteamApiKey { get; }
	public ISteamPlayerService SteamPlayerService { get; }
	public IServerInfoService ServerInfoService { get; }
	public ISteamUserService SteamUserService { get; }
}