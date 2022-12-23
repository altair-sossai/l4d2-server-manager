using L4D2ServerManager.Modules.AntiCheat.PlayerIp.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.PlayerIp.Services;

public interface IPlayerIpService
{
	PlayerIp AddOrUpdate(PlayerIpCommand command);
}