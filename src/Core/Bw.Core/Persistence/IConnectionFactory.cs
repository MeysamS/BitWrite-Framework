using System.Data.Common;

namespace Bw.Core.Persistence;

public interface IConnectionFactory : IDisposable
{
    Task<DbConnection> GetOrCreateConnection();
}
