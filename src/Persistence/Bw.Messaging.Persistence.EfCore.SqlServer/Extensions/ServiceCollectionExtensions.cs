using Bw.Core.Messaging;
using Bw.Extensions.Microsoft.DependencyInjection;
using Bw.Messaging.Persistence.EfCore.SqlServer.MessagePersistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bw.Messaging.Persistence.EfCore.SqlServer.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMsSqlMessagePersistence(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddValidatedOptions<MessagePersistenceOptions>(nameof(MessagePersistenceOptions));

        services.AddDbContext<MessagePersistenceDbContext>(
            (options) =>
            {
                // Todo
                using var scope = services.BuildServiceProvider().CreateScope();
                var msSqlOptions = scope.ServiceProvider.GetRequiredService<MessagePersistenceOptions>();

                options
                    .UseSqlServer(
                        msSqlOptions.ConnectionString,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(
                                msSqlOptions.MigrationAssembly
                                    ?? typeof(MessagePersistenceDbContext).Assembly.GetName().Name
                            );
                            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        }
                    );
                //.UseSnakeCaseNamingConvention();
            }
        );

        services.ReplaceScoped<IMessagePersistenceRepository, MsSqlMessagePersistenceRepository>();
    }
}

