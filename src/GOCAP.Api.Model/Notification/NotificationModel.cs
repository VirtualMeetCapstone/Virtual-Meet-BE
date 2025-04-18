using MongoDB.Bson;

namespace GOCAP.Api.Model;

public class NotificationModel
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public ActionType? ActionType { get; set; }
    public NotificationSourceModel? Source { get; set; }
    public NotificationActorModel? Actor { get; set; }
    public BsonDocument? Metadata { get; set; }
    public bool IsRead { get; set; } = false;
    public long ExpireTime { get; set; }
    public long CreateTime {  get; set; }
}
