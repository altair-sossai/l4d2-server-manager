using System.Net.Http;
using System.Text;
using System.Web.Http;
using L4D2ServerManager.FunctionApp.Errors;
using Microsoft.AspNetCore.Mvc;

namespace L4D2ServerManager.FunctionApp.Extensions;

public static class ErrorResultExtensions
{
    public static IActionResult ResponseMessageResult(this ErrorResult errorResult)
    {
        var stringContent = new StringContent(errorResult.ToJson(), Encoding.UTF8, "application/json");

        var response = new HttpResponseMessage(errorResult.StatusCode)
        {
            Content = stringContent
        };

        return new ResponseMessageResult(response);
    }
}