using System.Net;
using System.Net.Sockets;
using System.Text;
using L4D2ServerManager.Server;

namespace L4D2ServerManager.Players.Services;

public class PlayerService : IPlayerService
{
    private static readonly byte[] Request = {0xFF, 0xFF, 0xFF, 0xFF, 0x55, 0xFF, 0xFF, 0xFF, 0xFF};

    public IEnumerable<Player> GetPlayers(IServer server)
    {
        if (!server.VirtualMachine.IsOn)
            yield break;

        var ipAddress = IPAddress.Parse(server.VirtualMachine.IpAddress);
        var ipEndPoint = new IPEndPoint(ipAddress, Convert.ToUInt16(server.Port));
        using var udpClient = new UdpClient();
        udpClient.Send(Request, Request.Length, ipEndPoint);

        var asyncResult = udpClient.BeginReceive(null, null);
        asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));

        if (!asyncResult.IsCompleted)
            yield break;

        var bytes = udpClient.EndReceive(asyncResult, ref ipEndPoint);
        if (bytes.Length != 9 || bytes[4] != 0x41)
            yield break;

        bytes[4] = 0x55;
        udpClient.Send(bytes, bytes.Length, ipEndPoint);

        var memoryStream = new MemoryStream(udpClient.Receive(ref ipEndPoint));
        var binaryReader = new BinaryReader(memoryStream, Encoding.UTF8);
        memoryStream.Seek(4, SeekOrigin.Begin);
        binaryReader.ReadByte();

        var count = binaryReader.ReadByte();
        for (var i = 0; i < count; i++)
            yield return new Player(ref binaryReader);
    }
}