namespace L4D2ServerManager.Users.ValueObjects;

public class UserInfo
{
    public string? Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Steam { get; set; }
    public bool Admin { get; set; }
}