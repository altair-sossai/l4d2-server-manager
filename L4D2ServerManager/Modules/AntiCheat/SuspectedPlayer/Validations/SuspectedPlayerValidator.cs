using FluentValidation;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Validations;

public class SuspectedPlayerValidator : AbstractValidator<SuspectedPlayer>
{
    public SuspectedPlayerValidator()
    {
        RuleFor(r => r.PartitionKey)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.CommunityId)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.Name)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.PictureUrl)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.ProfileUrl)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.TotalHoursPlayed)
            .GreaterThanOrEqualTo(0);
    }
}