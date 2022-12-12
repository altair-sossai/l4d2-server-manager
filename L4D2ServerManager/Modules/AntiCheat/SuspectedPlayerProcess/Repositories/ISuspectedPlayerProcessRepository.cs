using Azure;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Repositories;

public interface ISuspectedPlayerProcessRepository
{
	Pageable<SuspectedPlayerProcess> GetAllProcesses(long communityId);
	void AddOrUpdate(IEnumerable<SuspectedPlayerProcess> processes);
}