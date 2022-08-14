using L4D2ServerManager.ServerInfo.ValueObjects;
using Refit;

namespace L4D2ServerManager.ServerInfo.Services;

public interface IServerInfoService
{
    [Get("/IGameServersService/GetServerList/v1")]
    Task<ResponseData<ServersInfo>> GetServerInfo([AliasAs("key")] string key, [AliasAs("filter")] string filter);
}