using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Services;

public interface ISuspectedPlayerPingService
{
	void Ping(long communityId, PingCommand command);
}