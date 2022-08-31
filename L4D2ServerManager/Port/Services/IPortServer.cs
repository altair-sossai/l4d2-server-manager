namespace L4D2ServerManager.Port.Services;

public interface IPortServer
{
    List<Port> GetPorts(string ip);
}