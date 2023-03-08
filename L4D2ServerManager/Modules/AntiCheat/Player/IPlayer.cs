namespace L4D2ServerManager.Modules.AntiCheat.Player;

public interface IPlayer
{
    public long CommunityId { get; set; }
    public string? SteamId { get; }
    public string? Steam3 { get; }
    public string? Name { get; set; }
    public string? PictureUrl { get; set; }
    public string? ProfileUrl { get; set; }
    public int TotalHoursPlayed { get; set; }
}