namespace L4D2ServerManager.Modules.ServerManager.Port.Services;

public interface IPortServer
{
    List<Port> GetPorts(string ip);
}