using System;
using Microsoft.EntityFrameworkCore;
using workstation_backend.Shared.Domain.Repositories;
using workstation_backend.Shared.Infrastructure.Persistence.Configuration;

namespace workstation_backend.Shared.Infrastructure.Persistence.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
 protected readonly WorkstationContext Context;

    protected BaseRepository(WorkstationContext context)
    {
        Context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(TEntity entity)
    {
        await Context.Set<TEntity>().AddAsync(entity);
        //dapper
    }

    /// <inheritdoc />
    public async Task<TEntity?> FindByIdAsync(Guid id)
    {
        return await Context.Set<TEntity>().FindAsync(id);
    }

    /// <inheritdoc />
    public void Update(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
    }

    /// <inheritdoc />
    public void Remove(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> ListAsync()
    {
        return await Context.Set<TEntity>().ToListAsync();
    }

}
