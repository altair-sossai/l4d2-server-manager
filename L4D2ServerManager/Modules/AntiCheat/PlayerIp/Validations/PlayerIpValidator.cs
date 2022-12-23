using FluentValidation;
using L4D2ServerManager.Infrastructure.Helpers;

namespace L4D2ServerManager.Modules.AntiCheat.PlayerIp.Validations;

public class PlayerIpValidator : AbstractValidator<PlayerIp>
{
	public PlayerIpValidator()
	{
		RuleFor(r => r.Ip)
			.NotNull()
			.NotEmpty()
			.Must(IpHelper.IsValidIpv4);

		RuleFor(r => r.CommunityId)
			.GreaterThan(0);
	}
}