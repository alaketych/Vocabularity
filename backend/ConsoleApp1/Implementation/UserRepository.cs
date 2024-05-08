using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Vocabularity.Core.Configuration;
using Vocabularity.Core.Implementation;
using Vocabularity.Service.User.Interfaces;

namespace Vocabularity.Service.User.Implementation
{
    public class UserRepository : Repository<Entities.User>, IUserRepository
    {
        private readonly CosmosConfig appSettings;

        public readonly CosmosClient cosmosClient;
        public readonly Container cosmosContainer;

        public override string DatabaseId => appSettings.DatabaseId;

        public override string ContainerId => appSettings.DatabaseContainer;

        public UserRepository(
            IOptions<CosmosConfig> appSettings,
            CosmosClient cosmosClient) : base(appSettings, cosmosClient)
        {
            this.appSettings = appSettings.Value;
            this.cosmosClient = cosmosClient;
            cosmosContainer = this.cosmosClient.GetContainer(DatabaseId, ContainerId);
        }
    }
}
