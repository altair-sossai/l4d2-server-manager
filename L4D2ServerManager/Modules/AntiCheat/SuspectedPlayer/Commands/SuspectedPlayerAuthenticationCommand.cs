namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayer.Commands;

public class SuspectedPlayerAuthenticationCommand
{
    public SuspectedPlayerAuthenticationCommand(string? accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
            return;

        var segments = accessToken.Replace("basic ", string.Empty, StringComparison.CurrentCultureIgnoreCase).Trim().Split(':', 2);

        CommunityId = long.TryParse(segments.FirstOrDefault(), out var communityId) ? communityId : 0;
        Secret = segments.LastOrDefault();
    }

    public long CommunityId { get; }
    public string? Secret { get; }
    public bool Valid => CommunityId > 0 && !string.IsNullOrEmpty(Secret);
}