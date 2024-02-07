using Ardalis.GuardClauses;
using Bw.Core.Messaging;
using Bw.Extensions.Microsoft.DependencyInjection;
using Bw.Messaging.Persistence.EfCore.Postgres.MessagePersistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bw.Messaging.Persistence.EfCore.Postgres.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPostgresMessagePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddValidatedOptions<MessagePersistenceOptions>(nameof(MessagePersistenceOptions));

        services.AddScoped<IMessagePersistenceConnectionFactory>(sp =>
        {
            var postgresOptions = sp.GetService<MessagePersistenceOptions>();
            Guard.Against.NullOrEmpty(postgresOptions?.ConnectionString);

            return new NpgsqlMessagePersistenceConnectionFactory(postgresOptions.ConnectionString);
        });

        services.AddDbContext<MessagePersistenceDbContext>(
            (options) =>
            {
                // Todo
                using var scope = services.BuildServiceProvider().CreateScope();
                var postgresOptions = scope.ServiceProvider.GetRequiredService<MessagePersistenceOptions>();

                options
                    .UseNpgsql(
                        postgresOptions.ConnectionString,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(
                                postgresOptions.MigrationAssembly
                                    ?? typeof(MessagePersistenceDbContext).Assembly.GetName().Name
                            );
                            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        }
                    )
                    .UseSnakeCaseNamingConvention();
            }
        );

        services.ReplaceScoped<IMessagePersistenceRepository, PostgresMessagePersistenceRepository>();
    }
}

