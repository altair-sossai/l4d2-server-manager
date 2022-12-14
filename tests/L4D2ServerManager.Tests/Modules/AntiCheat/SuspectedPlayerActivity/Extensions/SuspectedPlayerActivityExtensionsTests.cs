using FluentAssertions;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace L4D2ServerManager.Tests.Modules.AntiCheat.SuspectedPlayerActivity.Extensions;

[TestClass]
public class SuspectedPlayerActivityExtensionsTests
{
	[TestMethod]
	public void PingExpired_Expired()
	{
		var activity = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.SuspectedPlayerActivity
		{
			PingFocused = null,
			PingUnfocused = null
		};

		activity.PingExpired().Should().BeTrue();

		activity = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.SuspectedPlayerActivity
		{
			PingFocused = null,
			PingUnfocused = DateTime.UtcNow.AddMinutes(-6)
		};

		activity.PingExpired().Should().BeTrue();

		activity = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.SuspectedPlayerActivity
		{
			PingFocused = DateTime.UtcNow.AddMinutes(-6),
			PingUnfocused = null
		};

		activity.PingExpired().Should().BeTrue();

		activity = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.SuspectedPlayerActivity
		{
			PingFocused = DateTime.UtcNow.AddMinutes(-6),
			PingUnfocused = DateTime.UtcNow.AddMinutes(-6)
		};

		activity.PingExpired().Should().BeTrue();
	}

	[TestMethod]
	public void PingExpired_Unexpired()
	{
		var activity = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.SuspectedPlayerActivity
		{
			PingFocused = null,
			PingUnfocused = DateTime.UtcNow
		};

		activity.PingExpired().Should().BeFalse();

		activity = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.SuspectedPlayerActivity
		{
			PingFocused = DateTime.UtcNow,
			PingUnfocused = null
		};

		activity.PingExpired().Should().BeFalse();

		activity = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.SuspectedPlayerActivity
		{
			PingFocused = DateTime.UtcNow,
			PingUnfocused = DateTime.UtcNow
		};

		activity.PingExpired().Should().BeFalse();
	}

	[TestMethod]
	public void ProcessExpired_Expired()
	{
		var activity = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.SuspectedPlayerActivity
		{
			Process = null
		};

		activity.ProcessExpired().Should().BeTrue();

		activity = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.SuspectedPlayerActivity
		{
			Process = DateTime.UtcNow.AddMinutes(-6)
		};

		activity.ProcessExpired().Should().BeTrue();
	}

	[TestMethod]
	public void ProcessExpired_Unexpired()
	{
		var activity = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.SuspectedPlayerActivity
		{
			Process = DateTime.UtcNow
		};

		activity.ProcessExpired().Should().BeFalse();
	}

	[TestMethod]
	public void ScreenshotExpired_Expired()
	{
		var activity = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.SuspectedPlayerActivity
		{
			Screenshot = null
		};

		activity.ScreenshotExpired().Should().BeTrue();

		activity = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.SuspectedPlayerActivity
		{
			Screenshot = DateTime.UtcNow.AddMinutes(-6)
		};

		activity.ScreenshotExpired().Should().BeTrue();
	}

	[TestMethod]
	public void ScreenshotExpired_Unexpired()
	{
		var activity = new L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerActivity.SuspectedPlayerActivity
		{
			Screenshot = DateTime.UtcNow
		};

		activity.ScreenshotExpired().Should().BeFalse();
	}
}