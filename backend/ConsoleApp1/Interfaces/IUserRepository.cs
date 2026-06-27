using Vocabularity.Core.Interfaces;
using UserEntity = Vocabularity.Core.Entities.User;

namespace Vocabularity.Service.User.Interfaces;

public interface IUserRepository : IRepository<UserEntity>
{
}
