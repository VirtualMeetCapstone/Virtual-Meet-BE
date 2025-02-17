namespace GOCAP.Domain;

public class Room : DateTrackingBase
{
    public Guid OwnerId { get; set; }
    public User? Owner { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaximumMembers { get; set; }
    public List<Media>? Medias { get; set; }
    public List<MediaUpload>? MediaUploads { get; set; }
    public RoomStatusType? Status { get; set; }
    public IEnumerable<User> Members { get; set; } = [];
}
