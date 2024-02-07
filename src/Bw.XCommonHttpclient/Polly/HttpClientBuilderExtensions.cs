using Bw.XCommonHttpclient.Polly.PollyOptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bw.XCommonHttpclient.Polly;


public static class HttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddPolicyHandlers(this IHttpClientBuilder httpClientBuilder,
                                                                                                ILoggerFactory loggerFactory,
                                                                                                PolicyConfig policyOptions)
    {
        var retryLogger = loggerFactory.CreateLogger("PollyHttpRetryPoliciesLogger");
        var circuitBreakerLogger = loggerFactory.CreateLogger("PollyHttpCircuitBreakerPoliciesLogger");
        var circuitBreakerPolicyConfig = (ICircuitBreakerPolicyConfig)policyOptions;
        var retryPolicyConfig = (IRetryPolicyConfig)policyOptions;

        return httpClientBuilder.AddRetryPolicyHandler(retryLogger, retryPolicyConfig)
                                .AddCircuitBreakerHandler(circuitBreakerLogger, circuitBreakerPolicyConfig);
    }

    public static IHttpClientBuilder AddPolicyHandlers(this IHttpClientBuilder httpClientBuilder,
        PolicyConfig policyOptions,
          ILoggerFactory loggerFactory)
    {
        var retryLogger = loggerFactory.CreateLogger("PollyHttpRetryPoliciesLogger");
        var circuitBreakerLogger = loggerFactory.CreateLogger("PollyHttpCircuitBreakerPoliciesLogger");


        var circuitBreakerPolicyConfig = (ICircuitBreakerPolicyConfig)policyOptions;
        var retryPolicyConfig = (IRetryPolicyConfig)policyOptions;

        return httpClientBuilder.AddRetryPolicyHandler(retryLogger, retryPolicyConfig)
            .AddCircuitBreakerHandler(circuitBreakerLogger, circuitBreakerPolicyConfig);
    }

    public static IHttpClientBuilder AddRetryPolicyHandler(this IHttpClientBuilder httpClientBuilder, ILogger logger, IRetryPolicyConfig retryPolicyConfig)
    {
        return httpClientBuilder.AddPolicyHandler(HttpRetryPolicies.GetHttpRetryPolicy(logger, retryPolicyConfig));
    }

    public static IHttpClientBuilder AddCircuitBreakerHandler(this IHttpClientBuilder httpClientBuilder, ILogger logger, ICircuitBreakerPolicyConfig circuitBreakerPolicyConfig)
    {
        return httpClientBuilder.AddPolicyHandler(HttpCircuitBreakerPolicies.GetHttpCircuitBreakerPolicy(logger, circuitBreakerPolicyConfig));
    }
}
