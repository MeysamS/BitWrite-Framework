using Bw.Core.Persistence;
using System.Data;
using System.Data.Common;

namespace Bw.Persistence.Dapper.SqlServer;

public class DapperConnectionFactory : IConnectionFactory
{
    private readonly DbProviderFactory _dbProviderFactory;
    private readonly string _connectionString;

    public DapperConnectionFactory(DbProviderFactory dbProviderFactory, string connectionString)
    {
        _dbProviderFactory = dbProviderFactory ?? throw new ArgumentNullException(nameof(dbProviderFactory));
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<DbConnection> GetOrCreateConnection()
    {
        var connection = _dbProviderFactory.CreateConnection();
        if (connection == null)
            throw new InvalidOperationException("Failed to create a connection.");

        if (connection.State != ConnectionState.Open)
        {
            connection.ConnectionString = _connectionString;
            await connection.OpenAsync(); // Open the connection
        }

        return connection;
    }

    public void Dispose()
    {
        // Cleanup resources if needed
    }
}
