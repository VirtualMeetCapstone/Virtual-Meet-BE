namespace GOCAP.Services;

[RegisterService(typeof(ISystemService))]
internal class SystemService(IKafkaProducer _kafkaProducer) : ISystemService
{
    public async Task<OperationResult> SendSystemNotificationToAllUsersAsync(string subject, string content)
    {
        var metadata = new Dictionary<string, string>
        {
            { "Subject", subject },
            { "Content", content }
        };

        var notificationEvent = new NotificationEvent
        {
            Type = NotificationType.System,
            ActionType = ActionType.Add,
            Metadata = metadata
        };

        await _kafkaProducer.ProduceAsync(KafkaConstants.Topics.Notification, notificationEvent);

        return new OperationResult(true);
    }
}
