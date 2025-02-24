namespace GOCAP.Database;

[BsonCollection("SearchHistories")]
public class SearchHistoryEntity : EntityMongoBase
{
    public Guid? UserId { get; set; }
    public string Query { get; set; } = string.Empty;
}
