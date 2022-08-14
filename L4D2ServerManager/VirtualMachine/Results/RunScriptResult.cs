using Azure.ResourceManager.Compute.Models;

namespace L4D2ServerManager.VirtualMachine.Results;

public class RunScriptResult
{
    private RunScriptResult()
    {
    }

    public RunScriptResult(VirtualMachineRunCommandResult runCommandResult)
    {
        Output = runCommandResult.Value.FirstOrDefault()?.Message;
    }

    public string? Output { get; }
    public static RunScriptResult Empty => new();
}