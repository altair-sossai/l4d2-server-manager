using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Contexts.AzureTableStorage.Repositories;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Entities.Infrastructure;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerPing.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Repositories;

public class SuspectedPlayerActivityRepository : BaseTableStorageRepository<SuspectedPlayerActivity>, ISuspectedPlayerActivityRepository
{
    public SuspectedPlayerActivityRepository(IAzureTableStorageContext tableContext)
        : base("SuspectedPlayerActivity", tableContext)
    {
    }

    public SuspectedPlayerActivity? Find(long communityId)
    {
        return Find("shared", communityId.ToString());
    }

    public IEnumerable<SuspectedPlayerActivity> GetAllActivities()
    {
        return GetAll();
    }

    public void Ping(long communityId, PingCommand command)
    {
        Activity activity = command.Focused ? new PingFocusedActivity(communityId) : new PingUnfocusedActivity(communityId);

        TableClient.UpsertEntity(activity);
    }

    public void Process(long communityId)
    {
        var activity = new ProcessActivity(communityId);

        TableClient.UpsertEntity(activity);
    }

    public void Screenshot(long communityId)
    {
        var activity = new ScreenshotActivity(communityId);

        TableClient.UpsertEntity(activity);
    }

    public void FileCheckSuccess(long communityId)
    {
        var activity = new FileCheckSuccessActivity(communityId);

        TableClient.UpsertEntity(activity);
    }

    public void FileCheckFail(long communityId)
    {
        var activity = new FileCheckFailActivity(communityId);

        TableClient.UpsertEntity(activity);
    }

    public void Delete(long communityId)
    {
        Delete("shared", communityId.ToString());
    }
}