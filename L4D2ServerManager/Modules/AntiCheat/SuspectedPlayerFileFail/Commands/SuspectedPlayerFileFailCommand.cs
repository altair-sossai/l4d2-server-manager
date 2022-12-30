using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Enums;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerFileFail.Commands;

public class SuspectedPlayerFileFailCommand
{
	public string? File { get; set; }
	public FailReason Reason { get; set; }
}