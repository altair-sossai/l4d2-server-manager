using System;
using System.Threading.Tasks;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerScreenshot.Services;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerScreenshotFunction
{
    private readonly ISuspectedPlayerActivityRepository _suspectedPlayerActivityRepository;
    private readonly ISuspectedPlayerScreenshotService _suspectedPlayerScreenshotService;
    private readonly ISuspectedPlayerService _suspectedPlayerService;
    private readonly IUserService _userService;

    public SuspectedPlayerScreenshotFunction(IUserService userService,
        ISuspectedPlayerService suspectedPlayerService,
        ISuspectedPlayerScreenshotService suspectedPlayerScreenshotService,
        ISuspectedPlayerActivityRepository suspectedPlayerActivityRepository)
    {
        _userService = userService;
        _suspectedPlayerService = suspectedPlayerService;
        _suspectedPlayerScreenshotService = suspectedPlayerScreenshotService;
        _suspectedPlayerActivityRepository = suspectedPlayerActivityRepository;
    }

    [FunctionName(nameof(SuspectedPlayerScreenshotFunction) + "_" + nameof(GenerateUploadUrlAsync))]
    public async Task<IActionResult> GenerateUploadUrlAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players-screenshot/generate-upload-url")] HttpRequest httpRequest)
    {
        try
        {
            var accessToken = httpRequest.AuthorizationToken();
            var appId = httpRequest.AppId();
            var suspectedPlayer = _suspectedPlayerService.EnsureAuthentication(accessToken, appId);
            var url = await _suspectedPlayerScreenshotService.GenerateUploadUrlAsync(suspectedPlayer.CommunityId);

            _suspectedPlayerActivityRepository.Screenshot(suspectedPlayer.CommunityId);

            return new OkObjectResult(new { url });
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerScreenshotFunction) + "_" + nameof(GetAsync))]
    public async Task<IActionResult> GetAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players-screenshot/{communityId:long}")] HttpRequest httpRequest, long communityId)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheatManager);

            var parameters = httpRequest.GetQueryParameterDictionary();
            var skip = parameters.ContainsKey("skip") && int.TryParse(parameters["skip"], out var skipValue) ? skipValue : 0;
            var take = parameters.ContainsKey("take") && int.TryParse(parameters["take"], out var takeValue) ? takeValue : 100;
            var screenshots = await _suspectedPlayerScreenshotService.ScreenshotsAsync(communityId, skip, take);

            return new OkObjectResult(screenshots);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerScreenshotFunction) + "_" + nameof(DeleteAsync))]
    public async Task<IActionResult> DeleteAsync([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "suspected-players-screenshot/{communityId:long}")] HttpRequest httpRequest, long communityId)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheatManager);

            await _suspectedPlayerScreenshotService.DeleteAllScreenshotsAsync(communityId);

            return new OkResult();
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerScreenshotFunction) + "_" + nameof(DeleteOldScreenshotsAsync))]
    public async Task DeleteOldScreenshotsAsync([TimerTrigger("0 */10 * * * *")] TimerInfo timerInfo)
    {
        await _suspectedPlayerScreenshotService.DeleteOldScreenshotsAsync();
    }
}