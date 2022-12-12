using AutoMapper;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Profiles;

public class SuspectedPlayerPingProfile : Profile
{
	public SuspectedPlayerPingProfile()
	{
		CreateMap<PingCommand, SuspectedPlayerPing>();
	}
}