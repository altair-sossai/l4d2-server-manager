using AutoMapper;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata.Profiles;

public class SuspectedPlayerMetadataProfile : Profile
{
    public SuspectedPlayerMetadataProfile()
    {
        CreateMap<MetadataCommand, SuspectedPlayerMetadata>();
    }
}