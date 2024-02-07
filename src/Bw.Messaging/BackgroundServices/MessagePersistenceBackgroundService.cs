using Bw.Core.Messaging;
using Bw.Core.Types;
using Bw.Extensions.Web;
using Bw.Messaging.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bw.Messaging.BackgroundServices;


// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services
public class MessagePersistenceBackgroundService : BackgroundService
{
    private readonly ILogger<MessagePersistenceBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly MessagePersistenceOptions _options;
    private readonly IMachineInstanceInfo _machineInstanceInfo;

    public MessagePersistenceBackgroundService(
        ILogger<MessagePersistenceBackgroundService> logger,
        IServiceProvider serviceProvider,
        IHostApplicationLifetime lifetime,
        MessagePersistenceOptions options,
        IMachineInstanceInfo machineInstanceInfo)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _lifetime = lifetime;
        _options = options;
        _machineInstanceInfo = machineInstanceInfo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await _lifetime?.WaitForAppStartup(stoppingToken)!)
        {
            return;
        }

        _logger.LogInformation(
            "MessagePersistence Background Service is starting on client '{@ClientId}' and group '{@ClientGroup}'",
            _machineInstanceInfo.ClientId,
            _machineInstanceInfo.ClientGroup);

        await ProcessAsync(stoppingToken);
    }



    private async Task ProcessAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IMessagePersistenceService>();
                await service.ProcessAllAsync(stoppingToken);
            }

            var delay = _options.Interval is { }
            ? TimeSpan.FromSeconds((int)_options.Interval)
            : TimeSpan.FromSeconds(30);

            await Task.Delay(delay, stoppingToken);
        }
    }
}
