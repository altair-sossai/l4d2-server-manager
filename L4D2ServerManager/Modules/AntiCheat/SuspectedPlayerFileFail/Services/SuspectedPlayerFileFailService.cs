using AutoMapper;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Commands;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Repositories;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Services;

public class SuspectedPlayerFileFailService : ISuspectedPlayerFileFailService
{
	private readonly IMapper _mapper;
	private readonly ISuspectedPlayerFileFailRepository _suspectedPlayerFileFailRepository;

	public SuspectedPlayerFileFailService(IMapper mapper, ISuspectedPlayerFileFailRepository suspectedPlayerFileFailRepository)
	{
		_mapper = mapper;
		_suspectedPlayerFileFailRepository = suspectedPlayerFileFailRepository;
	}

	public void BatchOperation(long communityId, IEnumerable<SuspectedPlayerFileFailCommand> commands)
	{
		var files = _mapper.Map<List<SuspectedPlayerFileFail>>(commands);

		files.ForEach(file => file.CommunityId = communityId);

		if (files.Count == 0)
			return;

		_suspectedPlayerFileFailRepository.Delete(communityId);
		_suspectedPlayerFileFailRepository.AddOrUpdate(files);
	}
}