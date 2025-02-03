namespace GOCAP.Database;

[BsonCollection("UserMessages")]
public class UserMessageEntity : EntityMongoBase
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string? Message { get; set; }
    public DateTime? ReadAt { get; set; }
    public Guid ParentId { get; set; }
}
