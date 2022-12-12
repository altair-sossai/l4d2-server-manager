using FluentValidation;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Services;

public class SuspectedPlayerSecretService : ISuspectedPlayerSecretService
{
	private readonly IValidator<AddSuspectedPlayerSecretCommand> _addSuspectedValidator;
	private readonly ISuspectedPlayerSecretRepository _suspectedPlayerSecretRepository;
	private readonly IValidator<ValidateSecretCommand> _validateSecretValidator;
	private readonly IValidator<SuspectedPlayerSecret> _validator;

	public SuspectedPlayerSecretService(ISuspectedPlayerSecretRepository suspectedPlayerSecretRepository,
		IValidator<SuspectedPlayerSecret> validator,
		IValidator<AddSuspectedPlayerSecretCommand> addSuspectedValidator,
		IValidator<ValidateSecretCommand> validateSecretValidator)
	{
		_suspectedPlayerSecretRepository = suspectedPlayerSecretRepository;
		_validator = validator;
		_addSuspectedValidator = addSuspectedValidator;
		_validateSecretValidator = validateSecretValidator;
	}

	public bool Validate(ValidateSecretCommand command)
	{
		var result = _validateSecretValidator.ValidateAsync(command).Result;

		return result.IsValid;
	}

	public SuspectedPlayerSecret Add(AddSuspectedPlayerSecretCommand command)
	{
		_addSuspectedValidator.ValidateAndThrowAsync(command).Wait();

		var suspectedPlayer = new SuspectedPlayerSecret
		{
			CommunityId = command.CommunityId ?? 0
		};

		_validator.ValidateAndThrowAsync(suspectedPlayer).Wait();
		_suspectedPlayerSecretRepository.Add(suspectedPlayer);

		return suspectedPlayer;
	}
}