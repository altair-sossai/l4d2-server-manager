namespace L4D2ServerManager.Port;

public class Port
{
    public Port(int portNumber, int connectedPlayers)
    {
        PortNumber = portNumber;
        ConnectedPlayers = connectedPlayers;
    }

    public int PortNumber { get; }
    public int ConnectedPlayers { get; }
}