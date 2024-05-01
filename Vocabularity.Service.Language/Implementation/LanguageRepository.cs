using Microsoft.Azure.Cosmos;
using Vocabularity.Core;
using Vocabularity.Core.Implementation;
using Vocabularity.Service.Language.Interfaces;

namespace Vocabularity.Service.Language.Implementation;

public class LanguageRepository : Repository<Entities.Language>, ILanguageRepository
{
    public readonly CosmosClient cosmosClient;
    public readonly Container cosmosContainer;

    public override string DatabaseId => throw new NotImplementedException();

    public override string ContainerId => throw new NotImplementedException();

    public LanguageRepository(CosmosClient cosmosClient) : base(cosmosClient)
    {
        this.cosmosClient = cosmosClient;
        cosmosContainer = this.cosmosClient.GetContainer(DatabaseId, ContainerId);
    }

    public async Task<IEnumerable<Entities.Language>> GetLanguagesByUser(string userId)
    {
        var result = new List<Entities.Language>();
        var container = cosmosContainer.GetItemQueryIterator<Entities.Language>(new QueryDefinition("SELECT * FROM c"));

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
