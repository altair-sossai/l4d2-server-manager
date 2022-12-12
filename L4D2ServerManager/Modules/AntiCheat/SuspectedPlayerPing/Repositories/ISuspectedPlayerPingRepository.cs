namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Repositories;

public interface ISuspectedPlayerPingRepository
{
	SuspectedPlayerPing? Find(long communityId);
	void AddOrUpdate(SuspectedPlayerPing suspectedPlayerPing);
}