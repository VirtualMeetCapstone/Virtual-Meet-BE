using MongoDB.Bson;

namespace GOCAP.Database;

[BsonCollection("Notifications")]
public class NotificationEntity : EntityMongoBase
{
    [BsonRequired]
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    [MaxLength(AppConstants.MaxLengthDescription)]
    [BsonRequired]
    public string Content { get; set; } = string.Empty;
    public ActionType? ActionType { get; set; }
    [BsonRequired]
    public NotificationSourceEntity? Source { get; set; }
    [BsonIgnoreIfNull]
    public NotificationActorEntity? Actor { get; set; }
    [BsonIgnoreIfNull]
    public BsonDocument? Metadata { get; set; }
    public bool IsRead { get; set; }
    public long ExpireTime { get; set; }
}

public class NotificationSourceEntity
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
}

public class NotificationActorEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Media? Picture { get; set; }
}
