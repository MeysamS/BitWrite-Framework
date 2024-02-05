using System.Reflection;
using Bw.Core.Messaging.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bw.Messaging.Persistence.EfCore.Postgres.MessagePersistence;

public class MessagePersistenceDbContext : DbContext
{
    /// <summary>
    /// The default database schema.
    /// </summary>
    public const string DefaultSchema = "messaging";

    public DbSet<StoreMessage> StoreMessages => Set<StoreMessage>();

    public MessagePersistenceDbContext(DbContextOptions<MessagePersistenceDbContext> options)
    : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}