namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Commands;

public class ProcessCommand
{
    public string RowKey => ProcessName ?? "empty";
    public string? ProcessName { get; set; }
    public string? WindowTitle { get; set; }
    public string? FileName { get; set; }
}