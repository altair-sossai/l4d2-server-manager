using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;

public interface ISuspectedPlayerService
{
    SuspectedPlayer? GetSuspectedPlayer(string? steamId);
    IEnumerable<SuspectedPlayer> GetSuspectedPlayers();
    SuspectedPlayer AddOrUpdate(SuspectedPlayerCommand command);
    void Delete(string? steamId);
}