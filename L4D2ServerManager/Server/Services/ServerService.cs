using L4D2ServerManager.Server.Info.Context;
using L4D2ServerManager.Server.Info.ValueObjects;
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
        return new Server(this, virtualMachine, port);
    }

    public async Task<bool> IsRunningAsync(string ip, int port)
    {
        var serverInfo = await GetServerInfoAsync(ip, port);

        return serverInfo != null;
    }

    public async Task<ServerInfo?> GetServerInfoAsync(string ip, int port)
    {
        var serverInfoService = SteamContext.ServerInfoService;
        var responseData = await serverInfoService.GetServerInfo(SteamApiKey, $"addr\\{ip}:{port}");

        return responseData.Response?.Servers?.FirstOrDefault();
    }
}