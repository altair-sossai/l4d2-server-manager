using AutoMapper;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Services;

public class SuspectedPlayerPingService : ISuspectedPlayerPingService
{
    private readonly IMapper _mapper;
    private readonly ISuspectedPlayerPingRepository _suspectedPlayerPingRepository;

    public SuspectedPlayerPingService(IMapper mapper, ISuspectedPlayerPingRepository suspectedPlayerPingRepository)
    {
        _mapper = mapper;
        _suspectedPlayerPingRepository = suspectedPlayerPingRepository;
    }

    public void Ping(long communityId, PingCommand command)
    {
        var ping = _mapper.Map<SuspectedPlayerPing>(command);

        ping.CommunityId = communityId;

        _suspectedPlayerPingRepository.AddOrUpdate(ping);
    }
}