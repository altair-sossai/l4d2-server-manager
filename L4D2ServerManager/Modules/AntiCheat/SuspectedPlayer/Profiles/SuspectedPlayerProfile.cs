using AutoMapper;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Profiles.MappingActions;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Profiles;

public class SuspectedPlayerProfile : Profile
{
    public SuspectedPlayerProfile()
    {
        CreateMap<SuspectedPlayerCommand, SuspectedPlayer>()
            .AfterMap<ComplementSuspectedPlayerDataMappingAction>();
    }
}