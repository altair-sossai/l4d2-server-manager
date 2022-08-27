using System.Linq;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Users.Services;
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

    [FunctionName(nameof(UserFunction) + "_" + nameof(Get))]
    public IActionResult Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")] HttpRequest httpRequest)
    {
        _userService.EnsureAuthentication(httpRequest.AuthorizationToken());

        var users = _userService.GetUsers().Select(user => user.Info()).ToList();

        return new OkObjectResult(users);
    }

    [FunctionName(nameof(UserFunction) + "_" + nameof(GetById))]
    public IActionResult GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{userId}")] HttpRequest httpRequest,
        string userId)
    {
        _userService.EnsureAuthentication(httpRequest.AuthorizationToken());

        var user = _userService.GetUser(userId);
        if (user == null)
            return new NotFoundResult();

        return new OkObjectResult(user.Info());
    }
}