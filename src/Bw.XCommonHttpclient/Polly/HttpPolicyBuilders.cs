using Polly;
using Polly.Extensions.Http;

namespace Bw.XCommonHttpclient.Polly;

public static class HttpPolicyBuilders
{
    public static PolicyBuilder<HttpResponseMessage> GetBaseBuilder() =>
        HttpPolicyExtensions.HandleTransientHttpError();
}