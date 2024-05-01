using Microsoft.Azure.Cosmos;
using Vocabularity.Core;
using Vocabularity.Core.Implementation;
using Vocabularity.Service.Dictionary.Interfaces;

namespace Vocabularity.Service.Dictionary.Implementation;

public class DictionaryRepository : Repository<Entities.Dictionary>, IDictionaryRepository
{
    public readonly CosmosClient cosmosClient;
    public readonly Container cosmosContainer;

    public override string DatabaseId => throw new NotImplementedException();

    public override string ContainerId => throw new NotImplementedException();

    public DictionaryRepository(CosmosClient cosmosClient) : base(cosmosClient)
    {
        this.cosmosClient = cosmosClient;
        cosmosContainer = this.cosmosClient.GetContainer(DatabaseId, ContainerId);
    }

    public async Task<IEnumerable<Entities.Dictionary>> GetDictionaryByLanguage(string userId)
    {
        var result = new List<Entities.Dictionary>();
        var container = cosmosContainer.GetItemQueryIterator<Entities.Dictionary>(new QueryDefinition("SELECT * FROM c"));

        while (container.HasMoreResults)
        {
            foreach (var item in await container.ReadNextAsync())
            {
                result.Add(item);
            }
        }

        return result;
    }
}
