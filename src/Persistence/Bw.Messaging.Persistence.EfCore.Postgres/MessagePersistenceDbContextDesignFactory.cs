using Bw.Messaging.Persistence.EfCore.Postgres.MessagePersistence;
using Bw.Persistence.EfCore.Postgres;

namespace Bw.Messaging.Persistence.EfCore.Postgres;

public class MessagePersistenceDbContextDesignFactory : DbContextDesignFactoryBase<MessagePersistenceDbContext>
{
    public MessagePersistenceDbContextDesignFactory()
        : base("MessagePersistenceOptions:ConnectionString") { }
}
