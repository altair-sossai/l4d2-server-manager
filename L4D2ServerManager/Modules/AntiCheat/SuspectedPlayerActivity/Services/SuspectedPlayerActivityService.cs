using L4D2ServerManager.Contexts.Steam.Structures;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Extensions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Results;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Services;

public class SuspectedPlayerActivityService : ISuspectedPlayerActivityService
{
    private readonly ISuspectedPlayerActivityRepository _suspectedPlayerActivityRepository;
    private readonly ISuspectedPlayerRepository _suspectedPlayerRepository;

    public SuspectedPlayerActivityService(ISuspectedPlayerRepository suspectedPlayerRepository,
        ISuspectedPlayerActivityRepository suspectedPlayerActivityRepository)
    {
        _suspectedPlayerRepository = suspectedPlayerRepository;
        _suspectedPlayerActivityRepository = suspectedPlayerActivityRepository;
    }

    public CheckAntiCheatUsageResult CheckAntiCheatUsage(CheckAntiCheatUsageCommand command)
    {
        var suspectedPlayers = _suspectedPlayerRepository.GetSuspectedPlayers().Select(s => s.CommunityId).ToHashSet();
        var activities = _suspectedPlayerActivityRepository.GetAllActivities().ToDictionary();
        var result = new CheckAntiCheatUsageResult();

        foreach (var suspected in command.Suspecteds)
        {
            if (!SteamIdentifiers.TryParse(suspected, out var steamIdentifiers) || steamIdentifiers.CommunityId == null)
                continue;

            if (!suspectedPlayers.Contains(steamIdentifiers.CommunityId.Value))
                continue;

            if (activities.ContainsKey(steamIdentifiers.CommunityId.Value) && !activities[steamIdentifiers.CommunityId.Value].Expired())
                continue;

            result.Add(steamIdentifiers);
        }

        return result;
    }
}