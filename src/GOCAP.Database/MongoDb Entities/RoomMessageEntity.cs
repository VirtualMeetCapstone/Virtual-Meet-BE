namespace GOCAP.Database;

[BsonCollection("RoomMessages")]
public class RoomMessageEntity : EntityMongoBase
{
    public Guid RoomId { get; set; }
    public Guid SenderId { get; set; }
    public string? Message { get; set; }
    public long CreateTime { get; set; }
    public bool IsPinned { get; set; }
    public Guid ParentId { get; set; }
}
