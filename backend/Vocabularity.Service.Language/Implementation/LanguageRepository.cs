using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Vocabularity.Core;
using Vocabularity.Core.Configuration;
using Vocabularity.Core.Implementation;
using Vocabularity.Service.Language.Interfaces;

namespace Vocabularity.Service.Language.Implementation;

public class LanguageRepository : Repository<Entities.Language>, ILanguageRepository
{
    private readonly CosmosConfig appSettings;

    public readonly CosmosClient cosmosClient;
    public readonly Container cosmosContainer;

    public override string DatabaseId => appSettings.DatabaseId;

    public override string ContainerId => appSettings.DatabaseContainer;

    public LanguageRepository(
        IOptions<CosmosConfig> appSettings,
        CosmosClient cosmosClient) : base(appSettings, cosmosClient)
    {
        this.appSettings = appSettings.Value;
        this.cosmosClient = cosmosClient;
        cosmosContainer = this.cosmosClient.GetContainer(DatabaseId, ContainerId);
    }

    public async Task<IEnumerable<Entities.Language>> GetLanguagesByUser(string userId)
    {
        var result = new List<Entities.Language>();
        var container = cosmosContainer.GetItemQueryIterator<Entities.Language>(
            new QueryDefinition($"SELECT * FROM c WHERE c.User = '{userId}'"));

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
