namespace GOCAP.Database;

[BsonCollection("CommentReplies")]
public class CommentReplyEntity : EntityMongoBase
{
    public Guid ParentId { get; set; }
    public CommentAuthorEntity Author { get; set; } = new();
    public string Content { get; set; } = string.Empty;
    public List<string> Mentions { get; set; } = [];
    public Media? Media { get; set; }
}
