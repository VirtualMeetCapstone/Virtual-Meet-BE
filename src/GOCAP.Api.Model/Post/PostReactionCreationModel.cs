namespace GOCAP.Api.Model;

public class PostReactionCreationModel
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }
}
