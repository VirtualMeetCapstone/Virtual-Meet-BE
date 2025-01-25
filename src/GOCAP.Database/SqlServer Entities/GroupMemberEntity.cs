namespace GOCAP.Database;

[Table("GroupMembers")]
public class GroupMemberEntity : EntitySqlBase
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public long JoinedAt { get; set; }
    public GroupEntity? Group { get; set; }
    public UserEntity? User { get; set; }
}
