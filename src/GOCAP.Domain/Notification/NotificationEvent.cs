using MongoDB.Bson;

namespace GOCAP.Domain;

public class NotificationEvent
{
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public ActionType? ActionType { get; set; }
    public NotificationSource? Source { get; set; }
    public Guid ActorId { get; set; }
    public BsonDocument? Metadata { get; set; }
}
