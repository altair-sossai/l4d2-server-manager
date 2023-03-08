using System;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.Auth.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class UserFunction
{
    private readonly IUserService _userService;

    public UserFunction(IUserService userService)
    {
        _userService = userService;
    }

    [FunctionName(nameof(UserFunction) + "_" + nameof(Me))]
    public IActionResult Me([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "me")] HttpRequest httpRequest)
    {
        try
        {
            var user = _userService.EnsureAuthentication(httpRequest.AuthorizationToken());

            return new OkObjectResult(user.Info());
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }
}