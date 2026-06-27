using Microsoft.EntityFrameworkCore;
using Vocabularity.Core.Data;
using Vocabularity.Core.Interfaces;

namespace Vocabularity.Core.Implementation;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    protected readonly VocabularityDbContext Context;

    protected readonly DbSet<TEntity> DbSet;

    protected Repository(VocabularityDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public async IAsyncEnumerable<TEntity> GetAllAsync()
    {
        foreach (var item in await DbSet.ToListAsync())
        {
            yield return item;
        }
    }

    public async Task<TEntity?> GetByIdAsync(string id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task CreateAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is not null)
        {
            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
        }
    }
}
