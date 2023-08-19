using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vocabularity.Core.Domain.Interfaces
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<IList<T>> GetAllAsync();

        Task<T> GetIdAsync(int id);

        Task<T> AddAsync(T id);

        Task<T> UpdateAsync(T id);

        Task<bool> DeleteAsync(int id);
    }
}
