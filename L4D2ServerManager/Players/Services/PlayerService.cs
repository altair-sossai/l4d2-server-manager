using System.Net;
using System.Net.Sockets;
using System.Text;

namespace L4D2ServerManager.Players.Services;

public class PlayerService : IPlayerService
{
    private static readonly byte[] Request = { 0xFF, 0xFF, 0xFF, 0xFF, 0x55, 0xFF, 0xFF, 0xFF, 0xFF };

    public List<Player> GetPlayers(string ip, int port)
    {
        var players = new List<Player>();

        try
        {
            var ipAddress = IPAddress.Parse(ip);
            var ipEndPoint = new IPEndPoint(ipAddress, Convert.ToUInt16(port));
            using var udpClient = new UdpClient();
            udpClient.Client.SendTimeout = 5000;
            udpClient.Client.ReceiveTimeout = 5000;

            udpClient.Send(Request, Request.Length, ipEndPoint);

            var bytes = udpClient.Receive(ref ipEndPoint);
            if (bytes.Length != 9 || bytes[4] != 0x41)
                return players;

            bytes[4] = 0x55;
            udpClient.Send(bytes, bytes.Length, ipEndPoint);

            var memoryStream = new MemoryStream(udpClient.Receive(ref ipEndPoint));
            var binaryReader = new BinaryReader(memoryStream, Encoding.UTF8);
            memoryStream.Seek(4, SeekOrigin.Begin);
            binaryReader.ReadByte();

            var count = binaryReader.ReadByte();
            for (var i = 0; i < count; i++)
            {
                var player = new Player(ref binaryReader);

                players.Add(player);
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }

        return players;
    }
}