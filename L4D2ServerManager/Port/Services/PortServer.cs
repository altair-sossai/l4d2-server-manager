using L4D2ServerManager.Server.Services;
using Microsoft.Extensions.Configuration;

namespace L4D2ServerManager.Port.Services;

public class PortServer : IPortServer
{
    private readonly IConfiguration _configuration;
    private readonly IServerService _serverService;

    public PortServer(IConfiguration configuration,
        IServerService serverService)
    {
        _configuration = configuration;
        _serverService = serverService;
    }

    private string Ports => _configuration.GetValue<string>(nameof(Ports));

    public IEnumerable<Port> GetPorts(string ip)
    {
        foreach (var portNumber in Ports.Split(',', ';', ' ').Select(int.Parse))
        {
            var serverInfo = _serverService.GetServerInfoAsync(ip, portNumber).Result;
            var port = new Port(portNumber, serverInfo?.Players ?? 0);

            yield return port;
        }
    }
}