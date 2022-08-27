using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace L4D2ServerManager.FunctionApp.Extensions;

public static class HttpRequestDataExtensions
{
    public static async Task<T> DeserializeBodyAsync<T>(this HttpRequest httpRequest)
    {
        using var streamReader = new StreamReader(httpRequest.Body);
        var json = await streamReader.ReadToEndAsync();
        var t = JsonConvert.DeserializeObject<T>(json);

        return t;
    }

    public static string GetToken(this HttpRequest httpRequest)
    {
        return httpRequest.Headers["Authorization"].ToString();
    }
}