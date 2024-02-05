using Bw.Persistence.EfCore.Postgres;

namespace Bw.Messaging.Persistence.EfCore.Postgres.MessagePersistence;

public class NpgsqlMessagePersistenceConnectionFactory : NpgsqlConnectionFactory, IMessagePersistenceConnectionFactory
{
    public NpgsqlMessagePersistenceConnectionFactory(string connectionString)
        : base(connectionString) { }
}
