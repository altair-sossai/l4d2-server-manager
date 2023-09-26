using System.Runtime.Serialization;
using Azure;
using Azure.Data.Tables;
using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.Auth.Users.ValueObjects;

namespace L4D2ServerManager.Modules.Auth.Users;

public class User : ITableEntity
{
    private string? _serverCfgFile;

    public string Id
    {
        get => RowKey;
        set => RowKey = value;
    }

    public string? DisplayName { get; set; }
    public string? Steam { get; set; }
    public int AccessLevelValue { get; set; }

    [IgnoreDataMember]
    public AccessLevel AccessLevel
    {
        get => (AccessLevel)AccessLevelValue;
        set => AccessLevelValue = (int)value;
    }

    public string? Secret { get; set; }

    public string ServerCfgFile
    {
        get => string.IsNullOrEmpty(_serverCfgFile) ? "server.cfg" : _serverCfgFile;
        set => _serverCfgFile = value;
    }

    public string PartitionKey { get; set; } = "shared";
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public UserInfo Info()
    {
        return new UserInfo
        {
            Id = Id,
            DisplayName = DisplayName,
            Steam = Steam,
            AccessLevel = AccessLevel
        };
    }
}