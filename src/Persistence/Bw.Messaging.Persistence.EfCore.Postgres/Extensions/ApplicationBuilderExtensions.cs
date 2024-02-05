using Bw.Messaging.Persistence.EfCore.Postgres.MessagePersistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bw.Messaging.Persistence.EfCore.Postgres.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task UsePostgresPersistenceMessage(this IApplicationBuilder app, ILogger logger)
    {
        await app.ApplyDatabaseMigrations(logger);
    }

    private static async Task ApplyDatabaseMigrations(this IApplicationBuilder app, ILogger logger)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var messagePersistenceDbContext =
            serviceScope.ServiceProvider.GetRequiredService<MessagePersistenceDbContext>();

        logger.LogInformation("Applying persistence-message migrations...");

        await messagePersistenceDbContext.Database.MigrateAsync();

        logger.LogInformation("persistence-message migrations applied");
    }
}

