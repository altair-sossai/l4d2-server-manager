using L4D2ServerManager.Contexts.Steam;
using L4D2ServerManager.Contexts.Steam.ValueObjects;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine;

namespace L4D2ServerManager.Modules.ServerManager.Server.Services;

public class ServerService : IServerService
{
    private readonly ISteamContext _steamContext;

    public ServerService(ISteamContext steamContext)
    {
        _steamContext = steamContext;
    }

    public IServer GetByPort(IVirtualMachine virtualMachine, int port)
    {
        return new Server(this, virtualMachine, port);
    }

    public async Task<bool> IsRunningAsync(string ip, int port)
    {
        var serverInfo = await GetServerInfoAsync(ip, port);

        return serverInfo != null;
    }

    public async Task<ServerInfo?> GetServerInfoAsync(string ip, int port)
    {
        try
        {
            var serverInfoService = _steamContext.ServerInfoService;
            var responseData = await serverInfoService.GetServerInfo(_steamContext.SteamApiKey, $"addr\\{ip}:{port}");

            return responseData.Response?.Servers?.FirstOrDefault();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return null;
        }
    }
}