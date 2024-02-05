using System.Data.Common;

namespace Bw.Persistence.Abstraction;

public interface IConnectionFactory : IDisposable
{
    Task<DbConnection> GetOrCreateConnection();
}
