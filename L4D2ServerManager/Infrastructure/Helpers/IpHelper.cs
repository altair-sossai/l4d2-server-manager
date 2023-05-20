namespace L4D2ServerManager.Infrastructure.Helpers;

public static class IpHelper
{
    public static bool IsValidIpv4(string? ip)
    {
        ip = ip?.Trim();

        if (string.IsNullOrWhiteSpace(ip))
            return false;

        var values = ip.Split('.');

        return values.Length == 4 && values.All(r => byte.TryParse(r, out _));
    }
}