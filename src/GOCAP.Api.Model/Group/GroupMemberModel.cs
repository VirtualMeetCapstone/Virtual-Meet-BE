namespace GOCAP.Api.Model;

public class GroupMemberModel
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
}
