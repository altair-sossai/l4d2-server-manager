using Azure;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Repositories;

public interface ISuspectedPlayerFileFailRepository
{
	Pageable<SuspectedPlayerFileFail> GetAllFiles(long communityId);
	void AddOrUpdate(IEnumerable<SuspectedPlayerFileFail> files);
	void Delete(long communityId);
	void DeleteOldFiles();
}