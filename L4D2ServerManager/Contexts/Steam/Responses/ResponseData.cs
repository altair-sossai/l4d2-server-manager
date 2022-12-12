using System.Text.Json.Serialization;

namespace L4D2ServerManager.Contexts.Steam.Responses;

public class ResponseData<T>
	where T : class
{
	[JsonPropertyName("response")]
	public T? Response { get; set; }
}