using L4D2ServerManager.Contexts.Steam.Structures;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Results;

public class SuspectedPlayerResult
{
    private readonly SteamIdentifiers? _steamIdentifiers;

    public SuspectedPlayerResult(SteamIdentifiers steamIdentifiers)
    {
        _steamIdentifiers = steamIdentifiers;
    }

    public string? CommunityId => _steamIdentifiers?.CommunityId?.ToString();
    public string? SteamId => _steamIdentifiers?.SteamId;
    public string? Steam3 => _steamIdentifiers?.Steam3;
}