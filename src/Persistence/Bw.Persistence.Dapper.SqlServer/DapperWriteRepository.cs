using Bw.Core.Domain.Model.Identity;
using Bw.Core.Persistence;
using Dapper;
using System.Linq.Expressions;

namespace Bw.Persistence.Dapper.SqlServer;

public abstract class DapperWriteRepository<TEntity, TId> : IWriteRepository<TEntity, TId>
where TEntity : class, IHaveIdentity<TId>
{
    private readonly IConnectionFactory _connectionFactory;
    protected abstract string TableName { get; }

    public DapperWriteRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        using (var connection = await _connectionFactory.GetOrCreateConnection())
        {
            var id = await connection.InsertAsync(entity);
            return entity;
        }
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        using (var connection = await _connectionFactory.GetOrCreateConnection())
        {
            await connection.UpdateAsync(entity);
            return entity;
        }
    }

    public async Task DeleteRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken = default)
    {
        using (var connection = await _connectionFactory.GetOrCreateConnection())
        {
            await connection.DeleteAsync(entities);
        }
    }

    public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        using (var connection = await _connectionFactory.GetOrCreateConnection())
        {
            var entitiesToDelete = (await connection.GetListAsync<TEntity>(predicate)).ToList();
            await connection.DeleteAsync(entitiesToDelete);
        }
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        using (var connection = await _connectionFactory.GetOrCreateConnection())
        {
            await connection.DeleteAsync(entity);
        }
    }

    public async Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        using (var connection = await _connectionFactory.GetOrCreateConnection())
        {
            await connection.DeleteAsync<TEntity>(new { Id = id });
        }
    }


    // Other methods can be implemented based on your needs

    public void Dispose()
    {
        // Cleanup resources if needed
    }
}
