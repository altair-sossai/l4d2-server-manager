namespace L4D2ServerManager.Modules.ServerManager.Players.Services;

public interface IPlayerService
{
    List<Player> GetPlayers(string ip, int port);
}