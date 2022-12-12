using FluentValidation;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Validations;

public class ValidateSecretCommandValidator : AbstractValidator<ValidateSecretCommand>
{
	private readonly ISuspectedPlayerSecretRepository _suspectedPlayerSecretRepository;

	public ValidateSecretCommandValidator(ISuspectedPlayerSecretRepository suspectedPlayerSecretRepository)
	{
		_suspectedPlayerSecretRepository = suspectedPlayerSecretRepository;

		RuleFor(r => r.Steam3)
			.NotEmpty()
			.NotNull();

		RuleFor(r => r.CommunityId)
			.NotNull()
			.GreaterThan(0);

		RuleFor(r => r.Secret)
			.NotEmpty()
			.NotNull()
			.Length(40);

		RuleFor(r => r)
			.Must(BeValidCommunityIdAndSecret);
	}

	private bool BeValidCommunityIdAndSecret(ValidateSecretCommand command)
	{
		return command.CommunityId.HasValue
		       && !string.IsNullOrEmpty(command.Secret)
		       && _suspectedPlayerSecretRepository.Validate(command.CommunityId.Value, command.Secret);
	}
}