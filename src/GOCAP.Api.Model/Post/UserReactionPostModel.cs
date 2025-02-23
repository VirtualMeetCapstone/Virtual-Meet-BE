namespace GOCAP.Api.Model;

public class UserReactionPostModel
{
    public Guid PostId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public ReactionType ReactionType { get; set; }
}
