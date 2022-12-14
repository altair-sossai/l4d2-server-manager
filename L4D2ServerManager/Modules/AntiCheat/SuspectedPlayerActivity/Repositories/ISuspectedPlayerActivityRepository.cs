using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Repositories;

public interface ISuspectedPlayerActivityRepository
{
	void Ping(long communityId, PingCommand command);
	void Process(long communityId);
	void Screenshot(long communityId);
	void Delete(long communityId);
}