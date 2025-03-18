using GOCAP.Common;
using GOCAP.Domain;
using GOCAP.Services.Intention;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GOCAP.Messaging.Consumer;

public class NotificationConsumer
    (IOptions<KafkaSettings> _kafkaSettings,
     ILogger<NotificationConsumer> _logger,
     IServiceProvider _serviceProvider)
     : KafkaConsumerBase(_kafkaSettings, _logger, KafkaConstants.Topics.Notification)
{
    protected override async Task ProcessMessageAsync(string message)
    {
        var notificationEvent = JsonHelper.Deserialize<NotificationEvent>(message);
        if (notificationEvent == null)
        {
            _logger.LogWarning("[Kafka] Invalid notification event.");
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        try
        {
            await notificationService.HandleNotificationEvent(notificationEvent);
            _logger.LogInformation("[Kafka] Handled notification event successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Kafka] Error handling notification event.");
        }
    }
}
