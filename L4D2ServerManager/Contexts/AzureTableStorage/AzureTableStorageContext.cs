﻿using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;

namespace L4D2ServerManager.Contexts.AzureTableStorage;

public class AzureTableStorageContext : IAzureTableStorageContext
{
    private static readonly HashSet<string> CreatedTables = new();
    private readonly IConfiguration _configuration;
    private TableServiceClient? _tableServiceClient;

    public AzureTableStorageContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string ConnectionString => _configuration.GetValue<string>("AzureWebJobsStorage")!;
    private TableServiceClient TableServiceClient => _tableServiceClient ??= new TableServiceClient(ConnectionString);

    public async Task<TableClient> GetTableClient(string tableName)
    {
        var tableClient = TableServiceClient.GetTableClient(tableName);

        if (CreatedTables.Contains(tableName))
            return tableClient;

        await tableClient.CreateIfNotExistsAsync();
        CreatedTables.Add(tableName);

        return tableClient;
    }
}