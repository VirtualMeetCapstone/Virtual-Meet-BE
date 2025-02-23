namespace GOCAP.Api.Model;

public class UserReactionPostModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Media? Media { get; set; } = null;
    public ReactionType ReactionType { get; set; }
}
