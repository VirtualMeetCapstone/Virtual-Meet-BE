namespace GOCAP.Api.Model;
public class CommentCreationModel
{
    public Guid AuthorId { get; set; }
    public Guid? ParentId { get; set; }
    public required string Content { get; set; }
}
