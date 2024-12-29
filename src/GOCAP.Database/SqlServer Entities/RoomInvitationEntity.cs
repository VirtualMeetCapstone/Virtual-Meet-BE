namespace GOCAP.Database;

[Table("RoomInvitations")]
public class RoomInvitationEntity : EntitySqlBase
{
    public Guid RoomId { get; set; }
    public Guid InvitedUserId { get; set; }
    public long CreateTime { get; set; }
    public RoomEntity? Room { get; set; }
    public UserEntity? InvitedUser { get; set; }
}
