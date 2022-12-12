using L4D2ServerManager.Contexts.Steam.Helpers;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Commands;

public class ValidateSecretCommand
{
	public string? Steam3 { get; set; }
	public long? CommunityId => SteamIdHelper.Steam3ToCommunityId(Steam3 ?? string.Empty);
	public string? Secret { get; set; }
}