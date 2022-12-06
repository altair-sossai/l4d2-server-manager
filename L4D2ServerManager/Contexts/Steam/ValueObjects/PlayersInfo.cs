using System.Text.Json.Serialization;

namespace L4D2ServerManager.Contexts.Steam.ValueObjects;

public class PlayersInfo
{
    [JsonPropertyName("players")]
    public List<PlayerInfo?>? Players { get; set; }
}