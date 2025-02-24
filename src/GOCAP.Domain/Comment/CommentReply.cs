namespace GOCAP.Domain;

public class CommentReply : DateTrackingBase
{
    public Guid ParentId { get; set; }
    public CommentAuthor Author { get; set; } = new();
    public string Content { get; set; } = string.Empty;
    public List<string> Mentions { get; set; } = [];
    public Media? Media { get; set; } = null;
}
