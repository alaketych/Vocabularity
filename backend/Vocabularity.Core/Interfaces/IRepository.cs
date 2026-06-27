using Vocabularity.Core.Entities;

namespace Vocabularity.Core.Interfaces;

public interface IRepository<TEntity> where TEntity : Entity
{
    IAsyncEnumerable<TEntity> GetAllAsync();

    Task<TEntity?> GetByIdAsync(string id);

    Task CreateAsync(TEntity entity);

    Task UpdateAsync(TEntity entity);

    Task DeleteAsync(string id);
}
