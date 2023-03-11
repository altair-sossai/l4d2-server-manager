using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata.Services;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerMetadataFunction
{
    private readonly ISuspectedPlayerMetadataRepository _suspectedPlayerMetadataRepository;
    private readonly ISuspectedPlayerMetadataService _suspectedPlayerMetadataService;
    private readonly ISuspectedPlayerService _suspectedPlayerService;
    private readonly IUserService _userService;

    public SuspectedPlayerMetadataFunction(IUserService userService,
        ISuspectedPlayerService suspectedPlayerService,
        ISuspectedPlayerMetadataService suspectedPlayerMetadataService,
        ISuspectedPlayerMetadataRepository suspectedPlayerMetadataRepository)
    {
        _userService = userService;
        _suspectedPlayerService = suspectedPlayerService;
        _suspectedPlayerMetadataService = suspectedPlayerMetadataService;
        _suspectedPlayerMetadataRepository = suspectedPlayerMetadataRepository;
    }

    [FunctionName(nameof(SuspectedPlayerMetadataFunction) + "_" + nameof(GetAllMetadatas))]
    public IActionResult GetAllMetadatas([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suspected-players-metadata/{communityId:long}")] HttpRequest httpRequest, long communityId)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheatManager);

            var metadatas = _suspectedPlayerMetadataRepository.GetAllMetadatas(communityId);

            return new OkObjectResult(metadatas.OrderBy(o => o.Name).ToList());
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerMetadataFunction) + "_" + nameof(AddOrUpdate))]
    public async Task<IActionResult> AddOrUpdate([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "suspected-players-metadata")] HttpRequest httpRequest)
    {
        try
        {
            var accessToken = httpRequest.AuthorizationToken();
            var appId = httpRequest.AppId();
            var suspectedPlayer = _suspectedPlayerService.EnsureAuthentication(accessToken, appId);
            var commands = await httpRequest.DeserializeBodyAsync<List<MetadataCommand>>();

            _suspectedPlayerMetadataService.BatchOperation(suspectedPlayer.CommunityId, commands);

            return new OkResult();
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerMetadataFunction) + "_" + nameof(Delete))]
    public IActionResult Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "suspected-players-metadata/{communityId:long}")] HttpRequest httpRequest, long communityId)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheatManager);
            _suspectedPlayerMetadataRepository.Delete(communityId);

            return new OkResult();
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerMetadataFunction) + "_" + nameof(DeleteOldMetadatas))]
    public void DeleteOldMetadatas([TimerTrigger("0 */10 * * * *")] TimerInfo timerInfo)
    {
        _suspectedPlayerMetadataRepository.DeleteOldMetadatas();
    }
}