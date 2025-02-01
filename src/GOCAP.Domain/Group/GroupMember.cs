namespace GOCAP.Domain;

public class GroupMember : DomainBase
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public long JoinedAt { get; set; }
}


