using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Results;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerScreenshot.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Repositories;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerFunction
{
    private readonly IMapper _mapper;
    private readonly ISuspectedPlayerRepository _suspectedPlayerRepository;
    private readonly ISuspectedPlayerScreenshotService _suspectedPlayerScreenshotService;
    private readonly ISuspectedPlayerSecretRepository _suspectedPlayerSecretRepository;
    private readonly ISuspectedPlayerService _suspectedPlayerService;
    private readonly IUserService _userService;

    public SuspectedPlayerFunction(IMapper mapper,
        IUserService userService,
        ISuspectedPlayerService suspectedPlayerService,
        ISuspectedPlayerScreenshotService suspectedPlayerScreenshotService,
        ISuspectedPlayerRepository suspectedPlayerRepository,
        ISuspectedPlayerSecretRepository suspectedPlayerSecretRepository)
    {
        _mapper = mapper;
        _userService = userService;
        _suspectedPlayerService = suspectedPlayerService;
        _suspectedPlayerScreenshotService = suspectedPlayerScreenshotService;
        _suspectedPlayerRepository = suspectedPlayerRepository;
        _suspectedPlayerSecretRepository = suspectedPlayerSecretRepository;
    }

    [FunctionName(nameof(SuspectedPlayerFunction) + "_" + nameof(GetSuspectedPlayers))]
    public IActionResult GetSuspectedPlayers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players")] HttpRequest httpRequest)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheat);

            var suspectedPlayers = _suspectedPlayerRepository.GetSuspectedPlayers();
            var result = _mapper.Map<List<SuspectedPlayerResult>>(suspectedPlayers);

            return new OkObjectResult(result);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerFunction) + "_" + nameof(GetSuspectedPlayer))]
    public IActionResult GetSuspectedPlayer([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players/{communityId:long}")] HttpRequest httpRequest,
        long communityId)
    {
        try
        {
            var suspectedPlayer = _suspectedPlayerRepository.GetSuspectedPlayer(communityId);
            if (suspectedPlayer == null)
                return new NotFoundResult();

            var result = _mapper.Map<SuspectedPlayerResult>(suspectedPlayer);

            return new OkObjectResult(result);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerFunction) + "_" + nameof(AddOrUpdate))]
    public async Task<IActionResult> AddOrUpdate([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "suspected-players")] HttpRequest httpRequest)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheat);

            var command = await httpRequest.DeserializeBodyAsync<SuspectedPlayerCommand>();
            var suspectedPlayer = _suspectedPlayerService.AddOrUpdate(command);
            var result = _mapper.Map<SuspectedPlayerResult>(suspectedPlayer);

            return new OkObjectResult(result);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerFunction) + "_" + nameof(Delete))]
    public IActionResult Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "suspected-players/{communityId:long}")] HttpRequest httpRequest, long communityId)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheat);
            _suspectedPlayerRepository.Delete(communityId);
            _suspectedPlayerSecretRepository.Delete(communityId);
            _suspectedPlayerScreenshotService.DeleteAllScreenshots(communityId);

            return new OkResult();
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerFunction) + "_" + nameof(Sync))]
    public void Sync([TimerTrigger("0 */10 * * * *")] TimerInfo timerInfo)
    {
        _suspectedPlayerService.Sync();
    }
}