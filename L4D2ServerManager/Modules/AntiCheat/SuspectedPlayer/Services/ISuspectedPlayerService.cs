using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Services;

public interface ISuspectedPlayerService
{
	SuspectedPlayer? Find(string? account);
	SuspectedPlayer AddOrUpdate(SuspectedPlayerCommand command);
	SuspectedPlayer EnsureAuthentication(string accessToken);
	void Sync();
}