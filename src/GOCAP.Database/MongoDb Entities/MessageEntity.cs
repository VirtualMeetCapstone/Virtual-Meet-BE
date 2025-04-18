namespace GOCAP.Database;

[BsonCollection("Messages")]
public class MessageEntity : EntityMongoBase
{
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; }
    public List<Media> Attachments { get; set; } = [];
    public bool IsPinned { get; set; }
    public bool IsEdited { get; set; }
    public Guid ParentId { get; set; }
    public List<Guid> MentionedUserIds { get; set; } = []; 
    public List<Guid> SeenBy { get; set; } = [];
    
    [BsonIgnoreIfNull]
    public Guid? ReceiverId { get; set; }
    [BsonIgnoreIfNull]
    public Guid? GroupId { get; set; }
    [BsonIgnoreIfNull]
    public Guid? RoomId { get; set; }
    [BsonIgnoreIfNull]
    public List<Guid>? VisibleTo { get; set; } 
    
}