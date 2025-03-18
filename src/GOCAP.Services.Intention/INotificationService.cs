namespace GOCAP.Services.Intention;

public interface INotificationService : IServiceBase<Notification>
{
    Task HandleNotificationEvent(NotificationEvent notificationEvent);
}
