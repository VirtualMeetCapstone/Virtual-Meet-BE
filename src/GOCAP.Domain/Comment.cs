namespace GOCAP.Domain;

public class Comment : DateObjectBase
{
    public string? Content { get; set; }
    public Media? Media { get; set; }
    public int? LikeCount { get; set; }
    public Guid? ParentId { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }

}
