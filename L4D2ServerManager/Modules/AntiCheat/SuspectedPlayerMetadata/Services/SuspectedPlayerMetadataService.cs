using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata.Services;

public class SuspectedPlayerMetadataService : ISuspectedPlayerMetadataService
{
    private readonly IMapper _mapper;
    private readonly ISuspectedPlayerMetadataRepository _suspectedPlayerMetadataRepository;

    public SuspectedPlayerMetadataService(IMapper mapper, ISuspectedPlayerMetadataRepository suspectedPlayerMetadataRepository)
    {
        _mapper = mapper;
        _suspectedPlayerMetadataRepository = suspectedPlayerMetadataRepository;
    }

    public void BatchOperation(long communityId, IEnumerable<MetadataCommand> commands)
    {
        var metadatassCommands = commands.GroupBy(g => g.RowKey).Select(s => s.First());
        var metadatass = _mapper.Map<List<SuspectedPlayerMetadata>>(metadatassCommands);

        metadatass.ForEach(metadata => metadata.CommunityId = communityId);

        if (metadatass.Count == 0)
            throw new ValidationException("Informe ao menos um metadado", new List<ValidationFailure>());

        _suspectedPlayerMetadataRepository.AddOrUpdate(metadatass);
    }
}