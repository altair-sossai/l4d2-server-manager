namespace L4D2ServerManager.Extensions;

public static class StringExtensions
{
    public static string FirstLetterToLower(this string s)
    {
        if (string.IsNullOrEmpty(s))
            return string.Empty;

        return s.Length == 1 ? s.ToLower() : $"{char.ToLower(s[0])}{s[1..]}";
    }
}