using System.Text.RegularExpressions;
using L4D2ServerManager.VirtualMachine.Results;

namespace L4D2ServerManager.Server.Results;

public class GetRunningServersResult
{
    private const string Pattern = @"^\t\d+\.(\d+)\t\(\d+/\d+/\d+ \d+:\d+:\d+\).*$";

    public GetRunningServersResult(RunScriptResult result)
    {
        foreach (Match match in Regex.Matches(result.Output!, Pattern, RegexOptions.Multiline))
        {
            var port = int.Parse(match.Groups[1].Value);

            Ports.Add(port);
        }
    }

    public HashSet<int> Ports { get; } = new();
}