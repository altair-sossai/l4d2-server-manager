using L4D2ServerManager.Infrastructure.Extensions;

namespace L4D2ServerManager.Modules.ServerManager.Players;

public class Player
{
    public Player(ref BinaryReader binReader)
    {
        Index = binReader.ReadByte();
        Name = binReader.ReadNullTerminatedString();
        Score = binReader.ReadInt32();
        Duration = binReader.ReadSingle();
    }

    public byte Index { get; }
    public string Name { get; }
    public int Score { get; }
    public float Duration { get; }
}