using L4D2ServerManager.Contexts.Steam.Structures;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Extensions;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

public class SuspectedPlayerCommand
{
	private string? _account;
	private SteamIdentifiers _steamIdentifiers;

	public string? Account
	{
		get => _account;
		set
		{
			_account = value;
			SteamIdentifiers.TryParse(value ?? string.Empty, out _steamIdentifiers);
		}
	}

	public string? CustomUrl => this.CustomUrl();
	public long? CommunityId => _steamIdentifiers.CommunityId;
	public string? SteamId => _steamIdentifiers.SteamId;
	public string? Steam3 => _steamIdentifiers.Steam3;
}