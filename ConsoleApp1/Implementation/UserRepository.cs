using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Vocabularity.Core.Configuration;
using Vocabularity.Core.Implementation;
using Vocabularity.Service.User.Interfaces;

namespace Vocabularity.Service.User.Implementation
{
    public class UserRepository : Repository<Entities.User>, IUserRepository
    {
        private readonly AppSettings appSettings;

        public readonly CosmosClient cosmosClient;
        public readonly Container cosmosContainer;

        public override string DatabaseId => throw new NotImplementedException();

        public override string ContainerId => throw new NotImplementedException();

        public UserRepository(
            IOptions<AppSettings> appSettings,
            CosmosClient cosmosClient) : base(appSettings, cosmosClient)
        {
            this.appSettings = appSettings.Value;
            this.cosmosClient = cosmosClient;
            cosmosContainer = this.cosmosClient.GetContainer(DatabaseId, ContainerId);
        }
    }
}
