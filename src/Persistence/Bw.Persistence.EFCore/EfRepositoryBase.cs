using Bw.Core.Domain.Event;
using Bw.Core.Domain.Model.Identity;
using Bw.Persistence.EFCore.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Bw.Persistence.EFCore;

public class EfRepositoryBase<TDbContext, TEntity, TKey> :
    IEfRepository<TEntity, TKey>,
    IPageRepository<TEntity, TKey>
    where TEntity : class, IHaveIdentity<TKey>
    where TDbContext : DbContext
{

    protected readonly TDbContext DbContext;
    private readonly IAggregatesDomainEventsRequestStore _aggregatesDomainEventsRequestStore;
    protected readonly DbSet<TEntity> DbSet;

    public EfRepositoryBase(TDbContext dbContext, IAggregatesDomainEventsRequestStore aggregatesDomainEventsRequestStore)
    {
        DbContext = dbContext;
        _aggregatesDomainEventsRequestStore = aggregatesDomainEventsRequestStore;
        DbSet = dbContext.Set<TEntity>();
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var items = DbSet.Where(predicate).ToList();
        return DeleteRangeAsync(items, cancellationToken);
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var item = await DbSet.SingleOrDefaultAsync(x => x.Id!.Equals(id), cancellationToken);
        DbSet.Remove(item!);
    }

    public async Task DeleteRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await DeleteAsync(entity, cancellationToken);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    => await DbSet.Where(predicate).ToListAsync(cancellationToken);

    public Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default)
     => DbSet.SingleOrDefaultAsync(x => x.Id!.Equals(id), cancellationToken);

    public Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
     => DbSet.SingleOrDefaultAsync(predicate, cancellationToken);

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
     => await DbSet.ToListAsync(cancellationToken);

    public virtual IEnumerable<TEntity> GetInclude(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null)
    {
        IQueryable<TEntity> query = DbSet;

        if (includes is not null)
        {
            query = includes(query);
        }

        return query.AsEnumerable();
    }

    public virtual IEnumerable<TEntity> GetInclude(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null, bool withTracing = true)
    {
        IQueryable<TEntity> query = DbSet;

        if (includes != null)
        {
            query = includes(query);
        }

        query = query.Where(predicate);

        if (withTracing == false)
        {
            query = query.Where(predicate).AsNoTracking();
        }

        return query.AsEnumerable();
    }

    public virtual async Task<IEnumerable<TEntity>> GetIncludeAsync(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null)
    {
        IQueryable<TEntity> query = DbSet;

        if (includes is not null)
        {
            query = includes(query);
        }

        return await query.ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> GetIncludeAsync(
        Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null,
        bool withTracking = true)
    {
        IQueryable<TEntity> query = DbSet;

        if (includes is not null)
        {
            query = includes(query);
        }

        query = query.Where(predicate);

        if (withTracking == false)
        {
            query = query.Where(predicate).AsNoTracking();
        }

        return await query.ToListAsync();
    }

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = DbContext.Entry(entity);
        entry.State = EntityState.Modified;

        return Task.FromResult(entry.Entity);
    }
}
