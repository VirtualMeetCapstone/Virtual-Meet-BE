namespace GOCAP.Domain;

public class Room : DateTrackingBase
{
    public Guid OwnerId { get; set; }
    public PrivacyType Privacy { get; set; } 
    public User? Owner { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaximumMembers { get; set; }
    public List<Media>? Medias { get; set; } = null;
    public RoomStatusType? Status { get; set; }
    public string? Password { get; set; }
    public bool RequireApproval { get; set; }
    public string? WelcomeMessage { get; set; }
    public IEnumerable<User> Members { get; set; } = [];
    public List<Guid>? TaggedUserId { get; set; }
    public List<string>? HashTags { get; set; }
    public int OnlineCount { get; set; }
}
