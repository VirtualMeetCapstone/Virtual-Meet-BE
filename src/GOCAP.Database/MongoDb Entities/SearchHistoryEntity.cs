namespace GOCAP.Database;

[BsonCollection("RoomMessages")]
public class SearchHistoryEntity : EntityMongoBase
{
    public Guid? UserId { get; set; }
    public string Query { get; set; } = string.Empty;
}
