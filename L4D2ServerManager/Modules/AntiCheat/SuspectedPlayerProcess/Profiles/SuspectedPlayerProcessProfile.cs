using AutoMapper;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Profiles;

public class SuspectedPlayerProcessProfile : Profile
{
	public SuspectedPlayerProcessProfile()
	{
		CreateMap<ProcessCommand, SuspectedPlayerProcess>();
	}
}