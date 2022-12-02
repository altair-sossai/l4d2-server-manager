using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace L4D2ServerManager.Tests.Modules.AntiCheat.SuspectedPlayer.Extensions;

[TestClass]
public class SuspectedPlayerExtensionsTests
{
    [TestMethod]
    public void SteamId_76561198141521946_Altair()
    {
        var suspectedPlayer = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.SuspectedPlayer
        {
            SteamId = 76561198141521946
        };

        suspectedPlayer.SteamId.Should().Be(76561198141521946);
        suspectedPlayer.Steam3.Should().Be(181256218);
        suspectedPlayer.Steam.Should().Be(90628109);
    }

    [TestMethod]
    public void SteamId_76561198142731867_Bruna()
    {
        var suspectedPlayer = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.SuspectedPlayer
        {
            SteamId = 76561198142731867
        };

        suspectedPlayer.SteamId.Should().Be(76561198142731867);
        suspectedPlayer.Steam3.Should().Be(182466139);
        suspectedPlayer.Steam.Should().Be(91233069);
    }

    [TestMethod]
    public void SteamId_76561198048273306_Duqi()
    {
        var suspectedPlayer = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.SuspectedPlayer
        {
            SteamId = 76561198048273306
        };

        suspectedPlayer.SteamId.Should().Be(76561198048273306);
        suspectedPlayer.Steam3.Should().Be(88007578);
        suspectedPlayer.Steam.Should().Be(44003789);
    }
}