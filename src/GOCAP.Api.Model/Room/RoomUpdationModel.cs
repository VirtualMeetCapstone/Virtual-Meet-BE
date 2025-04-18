namespace GOCAP.Api.Model;

public class RoomUpdationModel
{
    public PrivacyType? Privacy { get; set; }
    public string Topic { get; set; } = "";
    public string? Description { get; set; }
    public string? Password { get; set; }
    public int MaximumMembers { get; set; }
    public List<Media>? Medias { get; set; } = null;
    public List<Guid>? TaggedUserId { get; set; }
    public List<string>? HashTags { get; set; }
}
