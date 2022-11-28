using L4D2ServerManager.Modules.Auth.Users.Enums;

namespace L4D2ServerManager.Modules.Auth.Users.ValueObjects;

public class UserInfo
{
    public string? Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Steam { get; set; }
    public AccessLevel AccessLevel { get; set; }
}