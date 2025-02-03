namespace GOCAP.Database;

[BsonCollection("UserComments")]
public class UserCommentEntity : EntityMongoBase
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string? Message { get; set; }
    public Guid ParentId { get; set; }
}
