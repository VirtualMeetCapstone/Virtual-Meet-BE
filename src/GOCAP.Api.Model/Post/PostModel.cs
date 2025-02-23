namespace GOCAP.Api.Model;

public class PostModel : EntityDateTrackingBase<Guid>
{
    public override Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public ICollection<Comment> Comments { get; set; } = [];
    public List<Media>? Medias { get; set; } = null;
    public PrivacyType? Privacy { get; set; }
    public int TotalReactions { get; set; }
    public Dictionary<string, int> ReactionCounts { get; set; } = new();
}
