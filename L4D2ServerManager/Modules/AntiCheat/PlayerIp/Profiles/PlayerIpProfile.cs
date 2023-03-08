using AutoMapper;
using L4D2ServerManager.Modules.AntiCheat.Player.Results;
using L4D2ServerManager.Modules.AntiCheat.PlayerIp.Commands;
using L4D2ServerManager.Modules.AntiCheat.PlayerIp.Profiles.MappingActions;
using L4D2ServerManager.Modules.AntiCheat.PlayerIp.Results;

namespace L4D2ServerManager.Modules.AntiCheat.PlayerIp.Profiles;

public class PlayerIpProfile : Profile
{
    public PlayerIpProfile()
    {
        CreateMap<PlayerIpCommand, PlayerIp>();

        CreateMap<PlayerIp, IpResult>();
        CreateMap<PlayerIp, PlayerResult>()
            .AfterMap<PlayerResultMappingAction>();
    }
}