using L4D2ServerManager.Contexts.Steam.Structures;

namespace L4D2ServerManager.Modules.AntiCheat.Player;

public class Player : IPlayer
{
    private long _communityId;
    private SteamIdentifiers _steamIdentifiers;

    public long CommunityId
    {
        get => _communityId;
        set
        {
            _communityId = value;
            SteamIdentifiers.TryParse(value.ToString(), out _steamIdentifiers);
        }
    }

    public string? SteamId => _steamIdentifiers.SteamId;
    public string? Steam3 => _steamIdentifiers.Steam3;
    public string? Name { get; set; }
    public string? PictureUrl { get; set; }
    public string? ProfileUrl { get; set; }
    public int TotalHoursPlayed { get; set; }
}