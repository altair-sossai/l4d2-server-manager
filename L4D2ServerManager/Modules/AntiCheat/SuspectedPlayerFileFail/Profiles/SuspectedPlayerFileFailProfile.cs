using AutoMapper;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Profiles;

public class SuspectedPlayerFileFailProfile : Profile
{
    public SuspectedPlayerFileFailProfile()
    {
        CreateMap<SuspectedPlayerFileFailCommand, SuspectedPlayerFileFail>();
    }
}