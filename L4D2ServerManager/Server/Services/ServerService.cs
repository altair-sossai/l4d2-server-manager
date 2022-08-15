using L4D2ServerManager.ServerInfo.Context;
using L4D2ServerManager.VirtualMachine;
using Microsoft.Extensions.Configuration;

namespace L4D2ServerManager.Server.Services;

public class ServerService : IServerService
{
    private readonly IConfiguration _configuration;

    public ServerService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string SteamApiKey => _configuration.GetValue<string>(nameof(SteamApiKey));

    public IServer GetByPort(IVirtualMachine virtualMachine, int port)
    {
        return new Server(virtualMachine, port);
    }

    public async Task<ServerInfo.ValueObjects.ServerInfo> GetServerInfoAsync(string ip, int port)
    {
        var serverInfoService = SteamContext.ServerInfoService;
        var responseData = await serverInfoService.GetServerInfo(SteamApiKey, $"addr\\{ip}:{port}");
        var serverInfo = responseData.Response?.Servers?.First();

        return serverInfo!;
    }
}