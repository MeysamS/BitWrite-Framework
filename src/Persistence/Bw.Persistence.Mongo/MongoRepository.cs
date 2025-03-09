using Bw.Core.Domain.Model.Identity;
using Bw.Persistence.Mongo.Abstraction;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace Bw.Persistence.Mongo;

public class MongoRepository<TEntity, TId> : IMongoRepository<TEntity, TId>
    where TEntity : class, IHaveIdentity<TId>
{
    private readonly IMongoDbContext _context;
    protected readonly IMongoCollection<TEntity> DbSet;


    public MongoRepository(IMongoDbContext context)
    {
        _context = context;
        DbSet = _context.GetCollection<TEntity>();
    }
    public void Dispose()
    {
        _context?.Dispose();
    }

    public Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return FindOneAsync(e => e.Id!.Equals(id), cancellationToken);
    }

    public Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Find(predicate).SingleOrDefaultAsync(cancellationToken: cancellationToken)!;
    }

    public async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Find(predicate).ToListAsync(cancellationToken: cancellationToken)!;
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsQueryable().ToListAsync(cancellationToken);
    }
    public IQueryable<TEntity> GetAllAsQueryableAsync()
    {
        IQueryable<TEntity> queryable = DbSet.AsQueryable();
        return queryable;
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var session = _context.GetSession();
        if (session == null)
            throw new InvalidOperationException("No active transaction session found.");

        await DbSet.InsertOneAsync(session, entity, new InsertOneOptions(), cancellationToken);

        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var session = _context.GetSession();
        if (session == null)
            throw new InvalidOperationException("No active transaction session found.");


        await DbSet.ReplaceOneAsync(session, e => e.Id!.Equals(entity.Id), entity, new ReplaceOptions(), cancellationToken);

        return entity;
    }

    public async Task DeleteRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var session = _context.GetSession(); // دریافت Session در زمان اجرا
        if (session == null)
            throw new InvalidOperationException("No active transaction session found.");


        var ids = entities.Select(e => e.Id).ToList();
        var filter = Builders<TEntity>.Filter.In(e => e.Id, ids);

        await DbSet.DeleteManyAsync(session, filter, null, cancellationToken);
    }

    public async Task DeleteAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var session = _context.GetSession();
        if (session == null)
            throw new InvalidOperationException("No active transaction session found.");

        await DbSet.DeleteOneAsync(session, predicate, null, cancellationToken);
    }


    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var session = _context.GetSession();
        if (session == null)
            throw new InvalidOperationException("No active transaction session found.");

        await DbSet.DeleteOneAsync(session, e => e.Id!.Equals(entity.Id), null, cancellationToken);
    }

    public async Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        var session = _context.GetSession();
        if (session == null)
            throw new InvalidOperationException("No active transaction session found.");

        await DbSet.DeleteOneAsync(session, e => e.Id!.Equals(id), null, cancellationToken);
    }
}
