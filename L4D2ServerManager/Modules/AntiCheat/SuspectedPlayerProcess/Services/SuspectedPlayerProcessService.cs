using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
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

    public void BatchOperation(long communityId, IEnumerable<ProcessCommand> commands)
    {
        var processessCommands = commands.GroupBy(g => g.RowKey).Select(s => s.First());
        var processess = _mapper.Map<List<SuspectedPlayerProcess>>(processessCommands);

        processess.ForEach(process => process.CommunityId = communityId);

        if (processess.Count == 0)
            throw new ValidationException("Informe ao menos um processo", new List<ValidationFailure>());

        _suspectedPlayerProcessRepository.AddOrUpdate(processess);
    }
}