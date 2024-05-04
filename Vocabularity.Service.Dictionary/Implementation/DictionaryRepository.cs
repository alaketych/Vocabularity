using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Vocabularity.Core.Configuration;
using Vocabularity.Core.Implementation;
using Vocabularity.Service.Dictionary.Interfaces;

namespace Vocabularity.Service.Dictionary.Implementation;

public class DictionaryRepository : Repository<Entities.Dictionary>, IDictionaryRepository
{
    private readonly CosmosConfig appSettings;

    public readonly CosmosClient cosmosClient;
    public readonly Container cosmosContainer;

    public override string DatabaseId => appSettings.DatabaseId;

    public override string ContainerId => appSettings.DatabaseContainer;

    public DictionaryRepository(
        CosmosClient cosmosClient,
        IOptions<CosmosConfig> appSettings) : base(appSettings, cosmosClient)
    {
        this.appSettings = appSettings.Value;
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
