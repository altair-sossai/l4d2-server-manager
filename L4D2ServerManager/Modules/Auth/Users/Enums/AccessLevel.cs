namespace L4D2ServerManager.Modules.Auth.Users.Enums;

[Flags]
public enum AccessLevel
{
	Servers = 1,
	VirtualMachine = 2,
	AntiCheat = 4, 
	AntiCheatManager = 8,
}