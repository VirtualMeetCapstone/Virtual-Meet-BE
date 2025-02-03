namespace GOCAP.Domain;

public class Room : DateObjectBase
{
    public Guid? OwnerId { get; set; }
    public User? Owner { get; set; }
    public string? Topic { get; set; }
    public string? Description { get; set; }
    public int MaximumMembers { get; set; }
    public string? Medias { get; set; } 
    public RoomStatusType? Status { get; set; }
    public IEnumerable<User> Members { get; set; } = [];
}
