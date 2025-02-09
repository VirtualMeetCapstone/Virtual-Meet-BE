namespace GOCAP.Domain;

public class Comment : DateObjectBase
{
    public Guid PostId { get; set; }
    public required CommentAuthor Author { get; set; }
    public required List<CommentContent> Content { get; set; }
    public Guid? ParentId { get; set; }
    public List<Guid> ReplyIds { get; set; } = [];
}