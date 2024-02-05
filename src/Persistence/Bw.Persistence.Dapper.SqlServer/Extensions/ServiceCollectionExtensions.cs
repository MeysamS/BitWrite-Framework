using Bw.Core.Domain.Model.Aggregate;
using Bw.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;

namespace Bw.Persistence.Dapper.SqlServer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDapper(this IServiceCollection services, Action<DapperOptions> configureOptions)
        {

            DapperOptions options = new DapperOptions();
            configureOptions?.Invoke(options);

            services.AddSingleton<IConnectionFactory>(provider =>
              new DapperConnectionFactory(SqlClientFactory.Instance, options.ConnectionString));

            //services.AddScoped(typeof(IReadRepository<,>), typeof(DapperReadRepository<,>));
            //services.AddScoped(typeof(IWriteRepository<,>), typeof(DapperWriteRepository<,>));

            services.AddScoped<IUnitOfWork, DapperUnitOfWork>();
            return services;

        }


        public static IServiceCollection AddDapperRepository<TEntity, TKey, TRepository>(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TEntity : class, IAggregate<TKey>
            where TRepository : class, IRepository<TEntity, TKey>
        {
            return services.RegisterService<IRepository<TEntity, TKey>, TRepository>(lifetime);
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
}
