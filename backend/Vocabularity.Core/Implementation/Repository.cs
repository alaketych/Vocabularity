using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Vocabularity.Core.Configuration;
using Vocabularity.Core.Interfaces;

namespace Vocabularity.Core.Implementation;

public abstract class Repository<TEntity> : IRepository<TEntity>, IDisposable where TEntity : Entity
{
    private readonly CosmosConfig _appSettings;

    private readonly Container _container;

    private readonly CosmosClient _cosmosClient;

    public abstract string DatabaseId { get; }

    public abstract string ContainerId { get; }

    public Repository(
        IOptions<CosmosConfig> appSettings,
        CosmosClient cosmosClient)
    {
        _appSettings = appSettings.Value;
        _cosmosClient = cosmosClient;
        _container = _cosmosClient.GetContainer(_appSettings.DatabaseId, _appSettings.DatabaseContainer);
    }

    public async IAsyncEnumerable<TEntity> GetAllAsync()
    {
        var result = _container.GetItemQueryIterator<TEntity>(new QueryDefinition("SELECT * FROM c"));
        while (result.HasMoreResults)
        {
            foreach (var item in await result.ReadNextAsync())
            {
                yield return item;
            }
        }
    }

    public async IAsyncEnumerable<TEntity> GetAllAsync(string partitionKey)
    {
        var result = _container.GetItemQueryIterator<TEntity>(new QueryDefinition("SELECT * FROM c"), null,
            new QueryRequestOptions 
            { 
                PartitionKey = new PartitionKey(partitionKey) 
            });

        while (result.HasMoreResults)
        {
            foreach (var item in await result.ReadNextAsync())
            {
                yield return item;
            }
        }
    }

    public async Task<TEntity> GetByIdAsync(string id, string partitionKey)
    {
        var itemResponse = await _container.ReadItemAsync<TEntity>(id, new PartitionKey(partitionKey));
        if (itemResponse != null)
        {
            return itemResponse;
        }

        return null;
    }

    public async Task CreateAsync(TEntity entity, string partitionKey)
    {
        var itemResponse = await _container.CreateItemAsync<TEntity>(entity, new PartitionKey(partitionKey));
        entity.Id = itemResponse.ActivityId;
    }

    public async Task UpdateAsync(TEntity entity, string partitionKey)
    {
        await _container.ReplaceItemAsync<TEntity>(entity, entity.Id, new PartitionKey(partitionKey));
    }

    public async Task DeleteAsync(string id, string partitionKey)
    {
        await _container.DeleteItemAsync<TEntity>(id, new PartitionKey(partitionKey));
    }

    public void Dispose()
    {
        _cosmosClient?.Dispose();
    }
}
