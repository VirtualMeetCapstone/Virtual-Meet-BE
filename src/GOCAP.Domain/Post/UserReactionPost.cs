namespace GOCAP.Domain;

public class UserReactionPost
{
    public Guid PostId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public ReactionType ReactionType { get; set; }
}
