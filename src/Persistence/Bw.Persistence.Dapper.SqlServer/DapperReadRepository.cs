using Bw.Core.Domain.Model.Identity;
using Bw.Core.Persistence;
using Bw.Persistence.Dapper.SqlServer.Model;
using Dapper;
using System.Data;
using System.Linq.Expressions;

namespace Bw.Persistence.Dapper.SqlServer;


public abstract class DapperReadRepository<TEntity, TId> : IReadRepository<TEntity, TId>
where TEntity : class, IHaveIdentity<TId>
{
    protected readonly IConnectionFactory _connectionFactory;
    protected abstract string TableName { get; }
    public DapperReadRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        using (var connection = await _connectionFactory.GetOrCreateConnection())
        {
            return await connection.QueryFirstOrDefaultAsync<TEntity>(
                       $"SELECT * FROM {TableName} WHERE Id = @Id",
                       new { Id = id }
                   );
        }
    }

    public async Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        using (var connection = await _connectionFactory.GetOrCreateConnection())
        {
            var query = $"SELECT * FROM {TableName}";
            var entities = await connection.QueryAsync<TEntity>(query);

            return entities.FirstOrDefault(predicate.Compile());
        }
    }

    public async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        using (var connection = await _connectionFactory.GetOrCreateConnection())
        {
            var query = $"SELECT * FROM {TableName}";
            var entities = await connection.QueryAsync<TEntity>(query);

            return entities.Where(predicate.Compile()).ToList();
        }
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using (var connection = await _connectionFactory.GetOrCreateConnection())
        {
            var query = $"SELECT * FROM {TableName}";
            var entities = await connection.QueryAsync<TEntity>(query);

            return entities.ToList();
        }
    }


    public async Task<PaginationResult<TEntity>> GetPaginatedData(int pageSize, int pageNumber, string sortOrderColumn, string sortOrderDirection)
    {
        var offset = GetOffset(pageSize, pageNumber);
        var orderBySql = BuildOrderBySqlUsingIntepolation(sortOrderColumn, sortOrderDirection);

        var query = $@"
        SELECT 
            COUNT(0) AS TotalRowCount,
            (
                SELECT * 
                FROM {TableName}
                ORDER BY {orderBySql}
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY
            ) AS Data";

        using (var connection = await _connectionFactory.GetOrCreateConnection())
        {
            var multi = await connection.QueryMultipleAsync(query, new { pageSize, offset, sortOrderColumn, sortOrderDirection });
            var totalRowCount = multi.Read<int>().Single();
            var gridDataRows = multi.Read<TEntity>().ToList();

            return new PaginationResult<TEntity> { TotalRowCount = totalRowCount, Rows = gridDataRows };
        }
    }

    /// Use string interpolation when order by columns are different SQL types - i.e in this example Name is varchar whilst create_date is a date.
    private static string BuildOrderBySqlUsingIntepolation(string sortOrderColumn, string sortOrderDirection)
    {
        string orderBy = $"{sortOrderColumn}";
        if (!string.IsNullOrEmpty(sortOrderDirection))
        {
            var sortOrder = "asc";
            if (sortOrderDirection == "desc")
            {
                sortOrder = "desc";
            }
            orderBy = $"{orderBy} {sortOrder}";
        }
        return orderBy;
    }
    private static int GetOffset(int pageSize, int pageNumber)
    {
        return (pageNumber - 1) * pageSize;
    }




    public async Task<IEnumerable<TEntity>> ExecuteQueryAsync(string sqlQuery, object? parameters = null, CancellationToken cancellationToken = default)
    {
        using (var connection = await _connectionFactory.GetOrCreateConnection())
        {
            var entities = await connection.QueryAsync<TEntity>(sqlQuery, parameters);
            return entities;
        }
    }


    // Other methods can be implemented based on your needs

    public void Dispose()
    {
        // Cleanup resources if needed
    }
}
