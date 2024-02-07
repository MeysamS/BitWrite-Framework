using System.Diagnostics.CodeAnalysis;
using Bw.XCommonHttpclient.Polly;
using Bw.XCommonHttpclient.Polly.PollyOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bw.XCommonHttpclient;
public static class HttpClientExtensions
{
    public static IServiceCollection AddCommonHttpClient<THttpSeperator,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THttpSeperatorImplementation>
    (this IServiceCollection services, IConfiguration configuration, string baseAddress)
    where THttpSeperator : class
    where THttpSeperatorImplementation : class, THttpSeperator, new()

    {
        services.AddSingleton<THttpSeperator, THttpSeperatorImplementation>();
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });

        var clientBuilder = services.AddHttpClient<ICommonHttpClient<THttpSeperator>, CommonHttpClient<THttpSeperator>>(options =>
        {
            options.BaseAddress = new Uri(baseAddress);
        });


        services.AddSingleton<CommonHttpClientFactory<THttpSeperator>>();

        var usePollyConfig = new UsePollyConfig();
        configuration.Bind($"{nameof(UsePollyConfig)}", usePollyConfig);
        if (usePollyConfig.Active)
            clientBuilder.AddPolicyHandlers(loggerFactory, usePollyConfig.PolicyConfig);

        return services;
    }

}
