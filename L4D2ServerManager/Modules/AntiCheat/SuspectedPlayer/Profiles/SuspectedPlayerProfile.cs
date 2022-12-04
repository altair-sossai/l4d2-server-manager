using AutoMapper;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Profiles.MappingActions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Results;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Profiles;

public class SuspectedPlayerProfile : Profile
{
    public SuspectedPlayerProfile()
    {
        CreateMap<SuspectedPlayerCommand, SuspectedPlayer>()
            .ForMember(dest => dest.CommunityId, opt => opt.MapFrom((src, dest) => Math.Max(src.CommunityId ?? 0, dest.CommunityId)))
            .AfterMap<ComplementSuspectedPlayerDataMappingAction>();

        CreateMap<SuspectedPlayer, SuspectedPlayerResult>();
    }
}