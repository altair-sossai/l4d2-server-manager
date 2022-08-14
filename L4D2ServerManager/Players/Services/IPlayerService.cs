using L4D2ServerManager.Server;

namespace L4D2ServerManager.Players.Services;

public interface IPlayerService
{
    IEnumerable<Player> GetPlayers(IServer server);
}