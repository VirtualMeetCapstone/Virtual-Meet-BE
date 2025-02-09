namespace GOCAP.Api.Model;

public class CommentCreationModel
{
    public Guid AuthorId { get; set; }
    public required List<CommentContentModel> Contents { get; set; }
}
