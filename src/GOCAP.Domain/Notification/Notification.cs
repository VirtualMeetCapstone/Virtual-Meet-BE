using MongoDB.Bson;

namespace GOCAP.Domain;

public class Notification : DateTrackingBase
{
    public List<Guid>? UserIds { get; set; }
    public NotificationType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public ActionType? ActionType { get; set; }
    public NotificationSource? Source { get; set; }
    public NotificationActor? Actor { get; set; }
    public BsonDocument? Metadata { get; set; }
    public bool IsRead { get; set; } = false;
    public long ExpireTime { get; set; }
}
