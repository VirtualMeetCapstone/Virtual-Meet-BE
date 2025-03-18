using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GOCAP.Messaging.Consumer;

public class KafkaConsumerService(
    IServiceProvider serviceProvider,
    ILogger<KafkaConsumerService> logger,
    IHostApplicationLifetime appLifetime) : IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<KafkaConsumerService> _logger = logger;
    private readonly IHostApplicationLifetime _appLifetime = appLifetime;
    private CancellationTokenSource? _cts;
    private IServiceScope? _scope;
    private Task? _userLoginTask;
    private Task? _notificationTask;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("⏳ Waiting for API to fully start...");

        _appLifetime.ApplicationStarted.Register(() =>
        {
            _logger.LogInformation("🚀 API started! Starting Kafka Consumers...");
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _scope = _serviceProvider.CreateScope();
            var userLoginConsumer = _scope.ServiceProvider.GetRequiredService<UserLoginConsumer>();
            var notificationConsumer = _scope.ServiceProvider.GetRequiredService<NotificationConsumer>();

            _userLoginTask = Task.Run(() => userLoginConsumer.StartAsync(_cts.Token), _cts.Token);
            _notificationTask = Task.Run(() => notificationConsumer.StartAsync(_cts.Token), _cts.Token);
        });

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛑 Stopping Kafka Consumers...");
        _cts?.Cancel();

        // Đợi các consumer dừng lại
        if (_userLoginTask != null) await _userLoginTask;
        if (_notificationTask != null) await _notificationTask;

        _scope?.Dispose();
    }
}