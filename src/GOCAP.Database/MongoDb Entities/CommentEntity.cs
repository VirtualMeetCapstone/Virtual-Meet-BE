namespace GOCAP.Database;

[BsonCollection("Comments")]
public class CommentEntity : EntityMongoBase
{
    public Guid PostId { get; set; }
    public required CommentAuthorEntity Author { get; set; }
    public required List<CommentContentEntity> Content { get; set; }
    public Guid? ParentId { get; set; }
    public List<Guid> ReplyIds { get; set; } = [];
}

public class CommentAuthorEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Picture { get; set; }
}

public class CommentContentEntity
{
    public required MediaType Type { get; set; }
    public required string Value { get; set; }
    public string? Thumbnail { get; set; }
}