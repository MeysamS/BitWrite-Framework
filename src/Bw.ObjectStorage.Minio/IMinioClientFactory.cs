using Microsoft.Extensions.Options;
using Minio;

namespace Bw.ObjectStorage.Minio;

public interface IMinioClientFactory
{
    MinioClient CreateClient();
    MinioClient CreateClient(string name);
}

public class MinioClientFactory : IMinioClientFactory
{
    private readonly IOptionsMonitor<MinioOptions> _optionsMonitor;

    public MinioClientFactory(IOptionsMonitor<MinioOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    public MinioClient CreateClient()
    {
        return CreateClient(Options.DefaultName);
    }

    public MinioClient CreateClient(string name)
    {
        var options = _optionsMonitor.Get(name);
        var client = new MinioClient()
            .WithEndpoint(options.Endpoint, options.Port)
            .WithSSL(options.UsedSslProtocol)
            .WithCredentials(options.AccessKey, options.SecretKey)
            .WithSessionToken(options.SessionToken);

        if (!string.IsNullOrEmpty(options.Region))
        {
            client.WithRegion(options.Region);
        }

        options.Configure?.Invoke((MinioClient)client);
        return (MinioClient)client.Build();
    }
}