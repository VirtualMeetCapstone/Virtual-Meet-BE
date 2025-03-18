
namespace GOCAP.Services;

[RegisterService(typeof(INotificationService))]
internal class NotificationService(
    INotificationRepository _repository,
    IUserRepository _userRepository,
    IFollowRepository _followRepository,
    IMapper _mapper,
    ILogger<NotificationService> _logger
    ) : ServiceBase<Notification, NotificationEntity>(_repository, _mapper, _logger), INotificationService
{
    private readonly IMapper _mapper = _mapper; 
    public async Task HandleNotificationEvent(NotificationEvent notificationEvent)
    {
        var actor = await _userRepository.GetByIdAsync(notificationEvent.ActorId);
        var notification = new Notification
        {
            Type = notificationEvent.Type,
            ActionType = notificationEvent.ActionType,
            Actor = new NotificationActor
            {
                Id = actor.Id,
                Name = actor.Name,
                Picture = actor.Picture,
            },
            Source = notificationEvent.Source
        };
        switch (notificationEvent.Type)
        {
            case NotificationType.Story:
            case NotificationType.Post:
            case NotificationType.Room:
                var listNotification = new List<NotificationEntity>();
                var recipientIds = await _followRepository.GetFollowersByUserIdAsync(notificationEvent.ActorId);
                foreach (var recipientId in recipientIds)
                {
                    var notificationMessage = new NotificationMessageBuilder()
                                                        .SetActor(actor.Name)
                                                        .SetAction(GetAction(notificationEvent.Type))
                                                        .SetTarget(GetTarget(notificationEvent.Type,                                                        notificationEvent.Source))
                                                        .Build();
                    notification.Content = notificationMessage;
                    notification.UserId = recipientId;
                    notification.InitCreation();
                    var entity = _mapper.Map<NotificationEntity>(notification);
                    listNotification.Add(entity);
                }
                await _repository.AddRangeAsync(listNotification);
                break;
            case NotificationType.Follow:
                break;
            case NotificationType.Comment:
                break;
            default: return;
        }
    }

    private static string GetAction(NotificationType type)
    {
        return type switch
        {
            NotificationType.Story => "posted",
            NotificationType.Post => "created",
            NotificationType.Room => "created",
            NotificationType.Follow => "followed",
            NotificationType.Comment => "commented on",
            _ => "performed an action"
        };
    }

    private static string GetTarget(NotificationType type, NotificationSource? source)
    {
        return type switch
        {
            NotificationType.Story => "a new story",
            NotificationType.Post => "a new post",
            NotificationType.Room => "a new room",
            NotificationType.Follow => "you",
            NotificationType.Comment => "your post",
            _ => "something"
        };
    }
}