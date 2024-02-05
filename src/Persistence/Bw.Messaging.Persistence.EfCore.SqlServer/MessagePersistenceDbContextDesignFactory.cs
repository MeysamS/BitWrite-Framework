using Bw.Messaging.Persistence.EfCore.SqlServer.MessagePersistence;
using Bw.Persistence.EFCore.SqlServer;

namespace Bw.Messaging.Persistence.EfCore.SqlServer;

public class MessagePersistenceDbContextDesignFactory : DbContextDesignFactoryBase<MessagePersistenceDbContext>
{
    public MessagePersistenceDbContextDesignFactory()
        : base("MessagePersistenceOptions:ConnectionString") { }
}
