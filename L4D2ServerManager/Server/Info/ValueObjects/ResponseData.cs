using System.Text.Json.Serialization;

namespace L4D2ServerManager.Server.Info.ValueObjects;

public class ResponseData<T>
    where T : class
{
    [JsonPropertyName("response")]
    public T? Response { get; set; }
}