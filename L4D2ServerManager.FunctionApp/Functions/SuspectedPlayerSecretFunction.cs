using System;
using AutoMapper;
using L4D2ServerManager.FunctionApp.Errors;
using L4D2ServerManager.FunctionApp.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Results;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace L4D2ServerManager.FunctionApp.Functions;

public class SuspectedPlayerSecretFunction
{
    private readonly IMapper _mapper;
    private readonly ISuspectedPlayerSecretService _suspectedPlayerSecretService;

    public SuspectedPlayerSecretFunction(IMapper mapper,
        ISuspectedPlayerSecretService suspectedPlayerSecretService)
    {
        _mapper = mapper;
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
}