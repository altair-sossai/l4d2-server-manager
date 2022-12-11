using L4D2ServerManager.Infrastructure.Helpers;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerProcess.Commands;

public class ProcessCommand
{
    public string RowKey => StringHelper.CreateMd5($"{ProcessName}.{FileName}.{Module}");
    public string? ProcessName { get; set; }
    public string? WindowTitle { get; set; }
    public string? FileName { get; set; }
    public string? Module { get; set; }
    public string? CompanyName { get; set; }
    public string? FileDescription { get; set; }
    public string? FileVersion { get; set; }
    public string? OriginalFilename { get; set; }
    public string? ProductName { get; set; }
}