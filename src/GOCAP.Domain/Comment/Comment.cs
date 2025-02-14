namespace GOCAP.Domain;

public class Comment : DateObjectBase
{
    public Guid PostId { get; set; }
    public required CommentAuthor Author { get; set; }
    public required string Content {  get; set; }
    public List<CommentContent>? Contents { get; set; }
    public Guid? ParentId { get; set; }
    public List<Guid> ReplyIds { get; set; } = [];
}