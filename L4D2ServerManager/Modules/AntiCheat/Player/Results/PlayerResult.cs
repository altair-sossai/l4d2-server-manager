namespace L4D2ServerManager.Modules.AntiCheat.Player.Results;

public class PlayerResult
{
	public string? CommunityId { get; set; }
	public string? SteamId { get; set; }
	public string? Steam3 { get; set; }
	public string? Name { get; set; }
	public string? PictureUrl { get; set; }
	public string? ProfileUrl { get; set; }
	public int TotalHoursPlayed { get; set; }
}