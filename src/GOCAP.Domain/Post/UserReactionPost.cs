namespace GOCAP.Domain;

public class UserReactionPost
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Media? Media { get; set; } = null;
    public ReactionType ReactionType { get; set; }
}
