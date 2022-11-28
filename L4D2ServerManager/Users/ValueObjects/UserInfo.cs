using L4D2ServerManager.Users.Enums;

namespace L4D2ServerManager.Users.ValueObjects;

public class UserInfo
{
    public string? Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Steam { get; set; }
    public AccessLevel AccessLevel { get; set; }
}