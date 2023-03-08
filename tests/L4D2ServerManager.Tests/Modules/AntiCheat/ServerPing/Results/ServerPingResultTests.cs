using FluentAssertions;
using L4D2ServerManager.Modules.AntiCheat.ServerPing.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace L4D2ServerManager.Tests.Modules.AntiCheat.ServerPing.Results;

[TestClass]
public class ServerPingResultTests
{
    [TestMethod]
    public void IsOn()
    {
        var serverPing = new L4D2ServerManager.Modules.AntiCheat.ServerPing.ServerPing
        {
            When = DateTime.UtcNow
        };

        var result = new ServerPingResult(serverPing);

        result.IsOn.Should().BeTrue();

        serverPing = new L4D2ServerManager.Modules.AntiCheat.ServerPing.ServerPing
        {
            When = DateTime.UtcNow.AddMinutes(-4)
        };

        result = new ServerPingResult(serverPing);

        result.IsOn.Should().BeTrue();

        serverPing = new L4D2ServerManager.Modules.AntiCheat.ServerPing.ServerPing
        {
            When = DateTime.UtcNow.AddMinutes(-40)
        };

        result = new ServerPingResult(serverPing);

        result.IsOn.Should().BeFalse();
    }
}