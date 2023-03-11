namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerMetadata.Commands;

public class MetadataCommand
{
    public string RowKey => Name!;
    public string? Name { get; set; }
    public string? Value { get; set; }
}