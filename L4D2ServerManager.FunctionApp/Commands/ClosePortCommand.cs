using System.Collections.Generic;

namespace L4D2ServerManager.FunctionApp.Commands;

public class ClosePortCommand
{
    public HashSet<string> AllowedIps { get; set; }
}