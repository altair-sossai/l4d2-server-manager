namespace L4D2ServerManager.Port.Services;

public interface IPortServer
{
    IEnumerable<Port> GetPorts(string ip);
}