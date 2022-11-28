using System.Text.Json.Serialization;

namespace L4D2ServerManager.Modules.ServerManager.Server.Info.ValueObjects;

public class ServersInfo
{
    [JsonPropertyName("servers")]
    public List<ServerInfo>? Servers { get; set; }
}