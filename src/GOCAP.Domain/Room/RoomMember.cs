namespace GOCAP.Domain;

public class RoomMember : DateObjectBase
{
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public ICollection<RoomMemberRole> RoomMemberRoles { get; set; } = [];
}
