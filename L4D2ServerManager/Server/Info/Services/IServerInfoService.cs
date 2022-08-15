using L4D2ServerManager.Server.Info.ValueObjects;
using Refit;

namespace L4D2ServerManager.Server.Info.Services;

public interface IServerInfoService
{
    [Get("/IGameServersService/GetServerList/v1")]
    Task<ResponseData<ServersInfo>> GetServerInfo([AliasAs("key")] string key, [AliasAs("filter")] string filter);
}