namespace L4D2ServerManager.Contexts.Steam.Services;

public interface ISteamIdService
{
	Task<long?> ResolveSteamIdAsync(string? customUrl);
}