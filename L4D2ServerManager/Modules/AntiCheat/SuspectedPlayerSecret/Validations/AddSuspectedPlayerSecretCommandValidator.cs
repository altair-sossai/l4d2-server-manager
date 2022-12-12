using FluentValidation;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Validations;

public class AddSuspectedPlayerSecretCommandValidator : AbstractValidator<AddSuspectedPlayerSecretCommand>
{
	private readonly ISuspectedPlayerRepository _suspectedPlayerRepository;
	private readonly ISuspectedPlayerSecretRepository _suspectedPlayerSecretRepository;

	public AddSuspectedPlayerSecretCommandValidator(ISuspectedPlayerRepository suspectedPlayerRepository,
		ISuspectedPlayerSecretRepository suspectedPlayerSecretRepository)
	{
		_suspectedPlayerRepository = suspectedPlayerRepository;
		_suspectedPlayerSecretRepository = suspectedPlayerSecretRepository;

		RuleFor(r => r.Steam3)
			.NotEmpty()
			.NotNull();

		RuleFor(r => r.CommunityId)
			.NotNull()
			.GreaterThan(0);

		When(w => w.CommunityId is > 0, () =>
		{
			RuleFor(r => r.CommunityId)
				.Must(BeAnSuspectedPlayer)
				.Must(BeUnregisteredSecret);
		});
	}

	private bool BeAnSuspectedPlayer(long? communityId)
	{
		return _suspectedPlayerRepository.Exists(communityId ?? 0);
	}

	private bool BeUnregisteredSecret(long? communityId)
	{
		return !_suspectedPlayerSecretRepository.Exists(communityId ?? 0);
	}
}