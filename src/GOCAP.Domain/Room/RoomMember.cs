namespace GOCAP.Domain;

public class RoomMember : DomainBase
{
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public long JoinedAt { get; set; }
    public ICollection<RoomMemberRole> RoomMemberRoles { get; set; } = [];
}
