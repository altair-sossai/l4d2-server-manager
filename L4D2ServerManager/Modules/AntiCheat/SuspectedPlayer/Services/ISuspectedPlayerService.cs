using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;

public interface ISuspectedPlayerService
{
    IEnumerable<SuspectedPlayer> GetSuspectedPlayers();
    Task<SuspectedPlayer?> AddAsync(AddSuspectedPlayerCommand command);
}