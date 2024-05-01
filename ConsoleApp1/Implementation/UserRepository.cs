using Microsoft.Azure.Cosmos;
using Vocabularity.Core;
using Vocabularity.Core.Implementation;
using Vocabularity.Service.User.Interfaces;

namespace Vocabularity.Service.User.Implementation
{
    public class UserRepository : Repository<Entities.User>, IUserRepository
    {
        public readonly CosmosClient cosmosClient;
        public readonly Container cosmosContainer;

        public override string DatabaseId => throw new NotImplementedException();

        public override string ContainerId => throw new NotImplementedException();

        public UserRepository(CosmosClient cosmosClient) : base(cosmosClient)
        {
            this.cosmosClient = cosmosClient;
            cosmosContainer = this.cosmosClient.GetContainer(DatabaseId, ContainerId);
        }
    }
}
