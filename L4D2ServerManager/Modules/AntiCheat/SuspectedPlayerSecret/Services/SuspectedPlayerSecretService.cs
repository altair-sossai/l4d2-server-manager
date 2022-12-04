using FluentValidation;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Services;

public class SuspectedPlayerSecretService : ISuspectedPlayerSecretService
{
    private readonly IValidator<AddSuspectedPlayerSecretCommand> _commandValidator;
    private readonly ISuspectedPlayerSecretRepository _suspectedPlayerSecretRepository;
    private readonly IValidator<SuspectedPlayerSecret> _validator;

    public SuspectedPlayerSecretService(ISuspectedPlayerSecretRepository suspectedPlayerSecretRepository,
        IValidator<SuspectedPlayerSecret> validator,
        IValidator<AddSuspectedPlayerSecretCommand> commandValidator)
    {
        _suspectedPlayerSecretRepository = suspectedPlayerSecretRepository;
        _validator = validator;
        _commandValidator = commandValidator;
    }

    public SuspectedPlayerSecret Add(AddSuspectedPlayerSecretCommand command)
    {
        _commandValidator.ValidateAndThrowAsync(command).Wait();

        var suspectedPlayer = new SuspectedPlayerSecret
        {
            CommunityId = command.CommunityId ?? 0
        };

        _validator.ValidateAndThrowAsync(suspectedPlayer).Wait();
        _suspectedPlayerSecretRepository.Add(suspectedPlayer);

        return suspectedPlayer;
    }
}