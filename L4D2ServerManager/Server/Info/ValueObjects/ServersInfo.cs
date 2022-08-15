using System.Text.Json.Serialization;

namespace L4D2ServerManager.Server.Info.ValueObjects;

public class ServersInfo
{
    [JsonPropertyName("servers")]
    public List<ServerInfo>? Servers { get; set; }
}