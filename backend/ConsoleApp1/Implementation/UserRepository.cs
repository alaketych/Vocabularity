using Vocabularity.Core.Data;
using Vocabularity.Core.Implementation;
using Vocabularity.Service.User.Interfaces;
using UserEntity = Vocabularity.Core.Entities.User;

namespace Vocabularity.Service.User.Implementation;

public class UserRepository : Repository<UserEntity>, IUserRepository
{
    public UserRepository(VocabularityDbContext context) : base(context)
    {
    }
}
