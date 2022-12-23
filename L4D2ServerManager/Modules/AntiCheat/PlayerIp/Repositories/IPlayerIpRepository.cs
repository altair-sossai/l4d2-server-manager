namespace L4D2ServerManager.Modules.AntiCheat.PlayerIp.Repositories;

public interface IPlayerIpRepository
{
	List<PlayerIp> GetAllPlayerIps(long communityId);
	List<PlayerIp> GetAllPlayersWithIp(string ip);
	void AddOrUpdate(PlayerIp playerIp);
	void Delete(long communityId);
	void DeleteOldIps();
}