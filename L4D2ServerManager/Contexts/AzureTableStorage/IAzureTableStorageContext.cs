using Azure.Data.Tables;

namespace L4D2ServerManager.Contexts.AzureTableStorage;

public interface IAzureTableStorageContext
{
	Task<TableClient> GetTableClient(string tableName);
}