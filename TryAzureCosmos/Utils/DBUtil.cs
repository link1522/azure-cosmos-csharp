using Microsoft.Azure.Cosmos;

namespace TryAzureCosmos.Utils
{
    internal class DBUtil
    {
        private CosmosClient _cosmosClient;
        private Database _database;

        public DBUtil(string connectionString, string databaseName)
        {
            _cosmosClient = new CosmosClient(connectionString);
            _database = _cosmosClient.GetDatabase(databaseName);
        }

        public async Task UpsertItem<T>(string containerName, T item, PartitionKey partitionKey)
        {
            var container = _database.GetContainer(containerName);
            await container.CreateItemAsync(item, partitionKey);
        }

        public async Task<T> ReadItem<T>(string containerName, string id, PartitionKey partitionKey)
        {
            try
            {
                var container = _database.GetContainer(containerName);
                var response = await container.ReadItemAsync<T>(id, partitionKey);
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception($"Item {id} not found");
            }
        }

        public async Task DeleteItem<T>(string containerName, string id, PartitionKey partitionKey)
        {
            var container = _database.GetContainer(containerName);
            await container.DeleteItemAsync<T>(id, partitionKey);
        }


        public async Task<List<T>> Query<T>(string containerName, string queryStr, Dictionary<string, object> parameters)
        {
            var container = _database.GetContainer(containerName);
            var query = new QueryDefinition(queryStr);
            foreach (var parameter in parameters)
            {
                query.WithParameter(parameter.Key, parameter.Value);
            }

            using var feed = container.GetItemQueryIterator<T>(query);

            var items = new List<T>();
            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync();
                foreach (T item in response)
                {
                    items.Add(item);
                }
            }

            return items;
        }
    }
}
