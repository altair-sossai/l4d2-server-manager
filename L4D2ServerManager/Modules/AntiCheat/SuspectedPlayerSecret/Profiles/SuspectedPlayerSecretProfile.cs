using AutoMapper;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Results;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Profiles;

public class SuspectedPlayerSecretProfile : Profile
{
    public SuspectedPlayerSecretProfile()
    {
        CreateMap<SuspectedPlayerSecret, SuspectedPlayerSecretResult>();
    }
}