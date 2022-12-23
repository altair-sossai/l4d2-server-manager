namespace L4D2ServerManager.Modules.AntiCheat.Player.Services;

public interface IPlayerService
{
	IPlayer Find(long communityId);
}