namespace Vocabularity.Core.Interfaces;

public interface IRepository<TEntity> : IDisposable where TEntity : Entity
{
    string DatabaseId { get; }
    string ContainerId { get; }

    IAsyncEnumerable<TEntity> GetAllAsync();

    IAsyncEnumerable<TEntity> GetAllAsync(string partitionKey);

    Task<TEntity> GetByIdAsync(string id, string partitionKey);

    Task CreateAsync(TEntity entity, string partitionKey);

    Task UpdateAsync(TEntity entity, string partitionKey);

    Task DeleteAsync(string id, string partitionKey);
}
