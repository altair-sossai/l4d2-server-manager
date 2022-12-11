using AutoMapper;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Services;

public class SuspectedPlayerProcessService : ISuspectedPlayerProcessService
{
    private readonly IMapper _mapper;
    private readonly ISuspectedPlayerProcessRepository _suspectedPlayerProcessRepository;

    public SuspectedPlayerProcessService(IMapper mapper, ISuspectedPlayerProcessRepository suspectedPlayerProcessRepository)
    {
        _mapper = mapper;
        _suspectedPlayerProcessRepository = suspectedPlayerProcessRepository;
    }

    public void AddOrUpdate(long communityId, IEnumerable<ProcessCommand> commands)
    {
        var processess = _mapper.Map<List<SuspectedPlayerProcess>>(commands);

        processess.ForEach(process => process.CommunityId = communityId);

        _suspectedPlayerProcessRepository.AddOrUpdate(processess);
    }
}