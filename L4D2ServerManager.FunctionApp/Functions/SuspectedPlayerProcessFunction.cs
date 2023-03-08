using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Services;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerProcessFunction
{
    private readonly ISuspectedPlayerActivityRepository _suspectedPlayerActivityRepository;
    private readonly ISuspectedPlayerProcessRepository _suspectedPlayerProcessRepository;
    private readonly ISuspectedPlayerProcessService _suspectedPlayerProcessService;
    private readonly ISuspectedPlayerService _suspectedPlayerService;
    private readonly IUserService _userService;

    public SuspectedPlayerProcessFunction(IUserService userService,
        ISuspectedPlayerService suspectedPlayerService,
        ISuspectedPlayerProcessService suspectedPlayerProcessService,
        ISuspectedPlayerProcessRepository suspectedPlayerProcessRepository,
        ISuspectedPlayerActivityRepository suspectedPlayerActivityRepository)
    {
        _userService = userService;
        _suspectedPlayerService = suspectedPlayerService;
        _suspectedPlayerProcessService = suspectedPlayerProcessService;
        _suspectedPlayerProcessRepository = suspectedPlayerProcessRepository;
        _suspectedPlayerActivityRepository = suspectedPlayerActivityRepository;
    }

    [FunctionName(nameof(SuspectedPlayerProcessFunction) + "_" + nameof(GetAllProcesses))]
    public IActionResult GetAllProcesses([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players-process/{communityId:long}")] HttpRequest httpRequest, long communityId)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheatManager);

            var processes = _suspectedPlayerProcessRepository.GetAllProcesses(communityId);

            return new OkObjectResult(processes.OrderBy(o => o.ProcessName).ToList());
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerProcessFunction) + "_" + nameof(AddOrUpdate))]
    public async Task<IActionResult> AddOrUpdate([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "suspected-players-process")] HttpRequest httpRequest)
    {
        try
        {
            var accessToken = httpRequest.AuthorizationToken();
            var appId = httpRequest.AppId();
            var suspectedPlayer = _suspectedPlayerService.EnsureAuthentication(accessToken, appId);
            var commands = await httpRequest.DeserializeBodyAsync<List<ProcessCommand>>();

            _suspectedPlayerProcessService.BatchOperation(suspectedPlayer.CommunityId, commands);
            _suspectedPlayerActivityRepository.Process(suspectedPlayer.CommunityId);

            return new OkResult();
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerProcessFunction) + "_" + nameof(Delete))]
    public IActionResult Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "suspected-players-process/{communityId:long}")] HttpRequest httpRequest, long communityId)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheatManager);
            _suspectedPlayerProcessRepository.Delete(communityId);

            return new OkResult();
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerProcessFunction) + "_" + nameof(DeleteOldProcesses))]
    public void DeleteOldProcesses([TimerTrigger("0 */10 * * * *")] TimerInfo timerInfo)
    {
        _suspectedPlayerProcessRepository.DeleteOldProcesses();
    }
}