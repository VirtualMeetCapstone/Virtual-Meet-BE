namespace GOCAP.Domain;

public class Comment : DateTrackingBase
{
    public Guid PostId { get; set; }
    public CommentAuthor? Author { get; set; }
    public string? Content {  get; set; }
    public Guid? ParentId { get; set; }
    public List<Guid> ReplyIds { get; set; } = [];
    public int ReplyCount { get; set; }
    public int TotalReactions { get; set; }
    public Dictionary<int, int> ReactionCounts { get; set; } = [];
}