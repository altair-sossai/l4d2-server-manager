using FluentValidation;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Validations;

public class SuspectedPlayerSecretValidator : AbstractValidator<SuspectedPlayerSecret>
{
    private readonly ISuspectedPlayerRepository _suspectedPlayerRepository;

    public SuspectedPlayerSecretValidator(ISuspectedPlayerRepository suspectedPlayerRepository)
    {
        _suspectedPlayerRepository = suspectedPlayerRepository;

        RuleFor(r => r.CommunityId)
            .NotNull()
            .GreaterThan(0);

        When(w => w.CommunityId > 0, () =>
        {
            RuleFor(r => r.CommunityId)
                .Must(BeAnSuspectedPlayer);
        });

        RuleFor(r => r.Secret)
            .NotNull()
            .NotEmpty()
            .Length(40);
    }

    private bool BeAnSuspectedPlayer(long communityId)
    {
        return _suspectedPlayerRepository.Exists(communityId);
    }
}