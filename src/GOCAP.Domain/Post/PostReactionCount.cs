namespace GOCAP.Domain;

public class PostReactionCount
{
    public Guid PostId { get; set; }
    public ReactionType Type { get; set; }
    public int Count { get; set; }
}
