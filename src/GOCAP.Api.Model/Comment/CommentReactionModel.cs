namespace GOCAP.Api.Model;

public class CommentReactionModel
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }
}
