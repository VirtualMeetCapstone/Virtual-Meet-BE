namespace GOCAP.Domain;

public class GroupMember : DateTrackingBase
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
}


