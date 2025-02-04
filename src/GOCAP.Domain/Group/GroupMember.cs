namespace GOCAP.Domain;

public class GroupMember : DateObjectBase
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
}


