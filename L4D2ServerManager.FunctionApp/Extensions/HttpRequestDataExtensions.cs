using System;
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

    public static void EnsureAuthentication(this HttpRequest httpRequest, string authKey)
    {
        var authorization = httpRequest.Headers["Authorization"].ToString();

        if (authorization != authKey)
            throw new Exception("Invalid Auth Key");
    }
}