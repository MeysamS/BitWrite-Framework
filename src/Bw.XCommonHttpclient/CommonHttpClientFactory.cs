using Microsoft.Extensions.DependencyInjection;

namespace Bw.XCommonHttpclient;
public class CommonHttpClientFactory<THttpSeperator> : ICommonHttpClientFactory<THttpSeperator>
{
    private readonly IServiceProvider _serviceProvider;
    public CommonHttpClientFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CommonHttpClient<THttpSeperator> Create()
    {
        return _serviceProvider.GetRequiredService<CommonHttpClient<THttpSeperator>>();
    }
}

public interface ICommonHttpClientFactory<THttpSeperator>
{
    CommonHttpClient<THttpSeperator> Create();
}
