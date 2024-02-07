using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Bw.ObjectStorage.Minio;

public static class MinioExtensions
{
    /// <summary>
    ///   Add default minio client with default configuration
    /// </summary>
    public static IServiceCollection AddMinio(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(nameof(MinioOptions), options =>
        {
            configuration.GetSection(nameof(MinioOptions)).Bind(options);
        });

        return services.AddMinio(_ => { });
    }

    private static IServiceCollection AddMinio(this IServiceCollection services, Action<MinioOptions> configure)
    {
        return services.AddMinio(Options.DefaultName, configure);
    }

    /// <summary>
    ///   Configure named minio client using Uri
    /// </summary>
    /// <example>s3://accessKey:secretKey@localhost:9000/region</example>
    public static IServiceCollection AddMinio(
           this IServiceCollection services,
           string name,
           Uri url,
           Action<MinioOptions>? configure = null)
    {
        return services.AddMinio(name, options =>
        {
            var credentials = url.UserInfo.Split(':');

            if (credentials.Length != 2)
            {
                throw new InvalidOperationException(
                             $"Invalid credentials format: {url.UserInfo}. s3://accessKey:secretKey@endpoint expected");
            }

            options.Endpoint = url.Authority;
            options.AccessKey = credentials[0];
            options.SecretKey = credentials[1];
            options.Region = url.AbsolutePath.TrimStart('/');
            configure?.Invoke(options);
        });
    }

    /// <summary>
    ///   Configure named minio client
    /// </summary>
    private static IServiceCollection AddMinio(
           this IServiceCollection services,
           string name,
           Action<MinioOptions> configure)
    {
        services.Configure(name, configure);
        services.TryAddSingleton<IMinioClientFactory, MinioClientFactory>();
        services.TryAddSingleton(sp => sp.GetRequiredService<IMinioClientFactory>().CreateClient(name));
        services.TryAddScoped<IMinioBlobStorage, MinioBlobStorage>();
        return services;
    }
}