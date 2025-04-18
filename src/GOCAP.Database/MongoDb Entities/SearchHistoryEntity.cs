namespace GOCAP.Database;

[BsonCollection("SearchHistories")]
public class SearchHistoryEntity : EntityMongoBase
{
    [BsonIgnoreIfDefault]
    [BsonIgnoreIfNull]
    public Guid? UserId { get; set; }
    public string Query { get; set; } = string.Empty;
}
