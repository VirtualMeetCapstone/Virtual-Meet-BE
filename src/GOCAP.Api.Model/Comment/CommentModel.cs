namespace GOCAP.Api.Model;

public class CommentModel
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public required CommentAuthor Author { get; set; }
    public required List<CommentContent> Content { get; set; }
    public Guid? ParentId { get; set; }
    public List<Guid> ReplyIds { get; set; } = [];
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }    
}
