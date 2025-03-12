using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GOCAP.Messaging;

public class KafkaConsumerService(
    IServiceProvider serviceProvider,
    ILogger<KafkaConsumerService> logger,
    IHostApplicationLifetime appLifetime) : IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<KafkaConsumerService> _logger = logger;
    private readonly IHostApplicationLifetime _appLifetime = appLifetime;
    private CancellationTokenSource? _cts;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("⏳ Waiting for API to fully start...");

        // Waiting for api completing starting.
        _appLifetime.ApplicationStarted.Register(() =>
        {
            _logger.LogInformation("🚀 API started! Starting Kafka Consumers...");
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _ = Task.Run(() => StartConsumersAsync(_cts.Token), _cts.Token);
        });

        return Task.CompletedTask;
    }

    private async Task StartConsumersAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var userLoginConsumer = scope.ServiceProvider.GetRequiredService<UserLoginConsumer>();

        await userLoginConsumer.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛑 Stopping Kafka Consumers...");
        _cts?.Cancel();
        return Task.CompletedTask;
    }
}
