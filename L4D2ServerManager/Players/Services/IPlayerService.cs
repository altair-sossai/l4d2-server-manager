namespace L4D2ServerManager.Players.Services;

public interface IPlayerService
{
    IEnumerable<Player> GetPlayers(string ip, int port);
}