namespace GOCAP.Database;

[BsonCollection("Comments")]
public class CommentEntity : EntityMongoBase
{
    public Guid PostId { get; set; }
    public CommentAuthorEntity? Author { get; set; }
    public string? Content { get; set; }
    public List<string> Mentions { get; set; } = [];
    public Guid? ParentId { get; set; }
    [BsonIgnore]
    public List<CommentEntity>? Replies { get; set; }
    [BsonIgnore]
    public int ReplyCount { get; set; }
}

public class CommentAuthorEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Picture { get; set; }
}
