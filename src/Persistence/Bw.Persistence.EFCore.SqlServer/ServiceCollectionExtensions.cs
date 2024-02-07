using Bw.Core.Domain.Event;
using Bw.Core.Domain.Event.Internal;
using Bw.Core.Domain.Model.Aggregate;
using Bw.Core.Persistence;
using Bw.Extensions.Microsoft.DependencyInjection;
using Bw.Persistence.EFCore;
using Bw.Persistence.EFCore.Abstraction;
using Bw.Persistence.EFCore.Converters;
using Bw.Persistence.EFCore.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bw.Persistence.EFCore.SqlServer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerDbContext<TDbContext>(
        this IServiceCollection services,
        Assembly? migrationAssembly = null,
        Action<DbContextOptionsBuilder>? builder = null)
        where TDbContext : DbContext, IDbFacadeResolver, IDomainEventContext
    {

        services.AddValidatedOptions<SqlServerOptions>(nameof(SqlServerOptions));
        services.AddDbContext<TDbContext>(
            (sp, option) =>
            {
                var sqlServeroptions = sp.GetRequiredService<SqlServerOptions>();
                Console.WriteLine($"connection string : {sqlServeroptions.ConnectionString}");
                option.UseSqlServer(sqlServeroptions.ConnectionString,
                    sqlOptions =>
                    {
                        var name = migrationAssembly?.GetName().Name
                        ?? typeof(TDbContext).Assembly.GetName().Name;

                        sqlOptions.MigrationsAssembly(name);
                        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    });

                // ref: https://andrewlock.net/series/using-strongly-typed-entity-ids-to-avoid-primitive-obsession/
                option.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector<long>>();

                option.AddInterceptors(
                    new AuditInterceptor(),
                    new SoftDeleteInterceptor(),
                    new ConcurrencyInterceptor()
                    );
                builder?.Invoke(option);
            });
        using var scope = services.BuildServiceProvider().CreateScope();
        var sqlopt = scope.ServiceProvider.GetRequiredService<SqlServerOptions>();
        services.AddScoped<IDbFacadeResolver>(provider => provider.GetService<TDbContext>()!);
        services.AddScoped<IDomainEventContext>(provider => provider.GetService<TDbContext>()!);
        services.AddScoped<IDomainEventsAccessor, EfDomainEventAccessor>();

        return services;
    }


    public static IServiceCollection AddSqlServerDbContext<TDbContext>(
      this IServiceCollection services,
      string connectionString,
      Assembly? migrationAssembly = null,
      Action<DbContextOptionsBuilder>? builder = null)
      where TDbContext : DbContext, IDbFacadeResolver, IDomainEventContext
    {

        services.AddValidatedOptions<SqlServerOptions>(nameof(SqlServerOptions));
        services.AddDbContext<TDbContext>(
            (sp, option) =>
            {
                option.UseSqlServer(connectionString,
                    sqlOptions =>
                    {
                        var name = migrationAssembly?.GetName().Name
                        ?? typeof(TDbContext).Assembly.GetName().Name;

                        sqlOptions.MigrationsAssembly(name);
                        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    });

                // ref: https://andrewlock.net/series/using-strongly-typed-entity-ids-to-avoid-primitive-obsession/
                option.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector<long>>();

                option.AddInterceptors(
                    new AuditInterceptor(),
                    new SoftDeleteInterceptor(),
                    new ConcurrencyInterceptor()
                    );
                builder?.Invoke(option);
            });
        using var scope = services.BuildServiceProvider().CreateScope();
        var sqlopt = scope.ServiceProvider.GetRequiredService<SqlServerOptions>();
        services.AddScoped<IDbFacadeResolver>(provider => provider.GetService<TDbContext>()!);
        services.AddScoped<IDomainEventContext>(provider => provider.GetService<TDbContext>()!);
        services.AddScoped<IDomainEventsAccessor, EfDomainEventAccessor>();

        return services;
    }


    public static IServiceCollection AddSqlServerCustomRepository(
        this IServiceCollection services,
        Type customRepositoryType)
    {
        services.Scan(scan =>

            scan.FromAssembliesOf(customRepositoryType)
            .AddClasses(classes => classes.AssignableTo(customRepositoryType))
            .As(typeof(IRepository<,>))
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(customRepositoryType))
            .As(typeof(IPageRepository<,>))
            .WithScopedLifetime()
        );
        return services;
    }

    public static IServiceCollection AddSqlServerRepository<TEntity, TKey, TRepository>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TEntity : class, IAggregate<TKey>
        where TRepository : class, IRepository<TEntity, TKey>
    {
        return services.RegisterService<IRepository<TEntity, TKey>, TRepository>(lifetime);
    }

    public static IServiceCollection AddUnitOfWork<TContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        bool registerGeneric = false)
        where TContext : EfDbContextBase
    {
        if (registerGeneric)
        {
            services.RegisterService<IUnitOfWork, EfUnitOfWork<TContext>>(lifetime);
        }
        return services.RegisterService<IEfUnitOfWork<TContext>, EfUnitOfWork<TContext>>(lifetime);
    }

    private static IServiceCollection RegisterService<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService
    {
        ServiceDescriptor serviceDescriptor = lifetime switch
        {
            ServiceLifetime.Singleton => ServiceDescriptor.Singleton<TService, TImplementation>(),
            ServiceLifetime.Scoped => ServiceDescriptor.Scoped<TService, TImplementation>(),
            ServiceLifetime.Transient => ServiceDescriptor.Transient<TService, TImplementation>(),
            _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
        };
        services.Add(serviceDescriptor);
        return services;
    }
}
