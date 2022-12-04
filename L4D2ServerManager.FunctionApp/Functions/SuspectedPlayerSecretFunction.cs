using System;
using AutoMapper;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Results;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Services;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerSecretFunction
{
    private readonly IMapper _mapper;
    private readonly ISuspectedPlayerSecretRepository _suspectedPlayerSecretRepository;
    private readonly ISuspectedPlayerSecretService _suspectedPlayerSecretService;
    private readonly IUserService _userService;

    public SuspectedPlayerSecretFunction(IMapper mapper,
        IUserService userService,
        ISuspectedPlayerSecretRepository suspectedPlayerSecretRepository,
        ISuspectedPlayerSecretService suspectedPlayerSecretService)
    {
        _mapper = mapper;
        _userService = userService;
        _suspectedPlayerSecretRepository = suspectedPlayerSecretRepository;
        _suspectedPlayerSecretService = suspectedPlayerSecretService;
    }

    [FunctionName(nameof(SuspectedPlayerSecretFunction) + "_" + nameof(Add))]
    public IActionResult Add([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "suspected-players-secret")] HttpRequest httpRequest)
    {
        try
        {
            var command = httpRequest.DeserializeBody<AddSuspectedPlayerSecretCommand>();
            var suspectedPlayerSecret = _suspectedPlayerSecretService.Add(command);
            var result = _mapper.Map<SuspectedPlayerSecretResult>(suspectedPlayerSecret);

            return new OkObjectResult(result);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }


    [FunctionName(nameof(SuspectedPlayerSecretFunction) + "_" + nameof(Validate))]
    public IActionResult Validate([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "suspected-players-secret/validate")] HttpRequest httpRequest)
    {
        try
        {
            var command = httpRequest.DeserializeBody<ValidateSecretCommand>();
            var valid = _suspectedPlayerSecretService.Validate(command);
            var result = new { valid };

            return new OkObjectResult(result);
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }

    [FunctionName(nameof(SuspectedPlayerSecretFunction) + "_" + nameof(Delete))]
    public IActionResult Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "suspected-players-secret/{communityId}")] HttpRequest httpRequest, long communityId)
    {
        try
        {
            _userService.EnsureAuthentication(httpRequest.AuthorizationToken(), AccessLevel.AntiCheat);
            _suspectedPlayerSecretRepository.Delete(communityId);

            return new OkResult();
        }
        catch (Exception exception)
        {
            return ErrorResult.Build(exception).ResponseMessageResult();
        }
    }
}