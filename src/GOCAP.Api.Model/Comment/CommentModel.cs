namespace GOCAP.Api.Model;
public class CommentModel
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public required CommentAuthor Author { get; set; }
    public required string Content { get; set; }
    public Guid? ParentId { get; set; }
    public List<Guid> Replies { get; set; } = [];
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
    public int ReplyCount { get; set; }
    public int TotalReactions { get; set; }
    public Dictionary<int, int> ReactionCounts { get; set; } = [];
}
