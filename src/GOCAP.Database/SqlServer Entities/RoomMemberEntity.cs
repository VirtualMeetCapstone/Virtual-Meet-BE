namespace GOCAP.Database;

[Table("RoomMembers")]
public class RoomMemberEntity : EntitySqlBase
{
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public long JoinedAt { get; set; }
    public IEnumerable<RoomMemberRoleEntity> RoomMemberRoles { get; set; } = [];
    public RoomEntity? Room { get; set; }
    public UserEntity? User { get; set; }
}
