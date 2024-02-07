using Bw.Core.Persistence;
using System.Data;

namespace Bw.Persistence.Dapper.SqlServer;

public class DapperUnitOfWork : IUnitOfWork
{
    private readonly IConnectionFactory _connectionFactory;
    private IDbTransaction? _transaction = null;

    public DapperUnitOfWork(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }


    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var connection = await _connectionFactory.GetOrCreateConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        _transaction = connection.BeginTransaction();
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            _transaction.Commit();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        if (_transaction != null)
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }
}
