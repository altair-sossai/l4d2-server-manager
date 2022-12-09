using System;
using System.Threading.Tasks;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerScreenshot.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerScreenshotFunction
{
    private readonly ISuspectedPlayerScreenshotService _suspectedPlayerScreenshotService;
    private readonly ISuspectedPlayerService _suspectedPlayerService;

    public SuspectedPlayerScreenshotFunction(ISuspectedPlayerService suspectedPlayerService,
        ISuspectedPlayerScreenshotService suspectedPlayerScreenshotService)
    {
        _suspectedPlayerService = suspectedPlayerService;
        _suspectedPlayerScreenshotService = suspectedPlayerScreenshotService;
    }

    [FunctionName(nameof(SuspectedPlayerScreenshotFunction) + "_" + nameof(GenerateUploadUrlAsync))]
    public async Task<IActionResult> GenerateUploadUrlAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players-Screenshot/generate-upload-url")] HttpRequest httpRequest)
    {
        try
        {
            var suspectedPlayer = _suspectedPlayerService.EnsureAuthentication(httpRequest.AuthorizationToken());
            var url = await _suspectedPlayerScreenshotService.GenerateUploadUrlAsync(suspectedPlayer.CommunityId);

            return new OkObjectResult(new { url });
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