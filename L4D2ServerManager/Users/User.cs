using Azure;
using L4D2ServerManager.Users.Enums;
using L4D2ServerManager.Users.ValueObjects;
using Microsoft.WindowsAzure.Storage.Table;
using ITableEntity = Azure.Data.Tables.ITableEntity;

namespace L4D2ServerManager.Users;

public class User : ITableEntity
{
    public string Id
    {
        get => RowKey;
        set => RowKey = value;
    }

    public string? DisplayName { get; set; }
    public string? Steam { get; set; }
    public int AccessLevelValue { get; set; }

    [IgnoreProperty]
    public AccessLevel AccessLevel
    {
        get => (AccessLevel)AccessLevelValue;
        set => AccessLevelValue = (int)value;
    }

    public string? Secret { get; set; }
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