namespace GOCAP.Api.Model;

public class RoomModel : EntityDateTrackingBase<Guid>
{
    public override Guid Id { get; set; }
    public Guid? OwnerId { get; set; }
    public RoomMemberModel? Owner { get; set; }
    public PrivacyType Privacy { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaximumMembers { get; set; }
    public List<Media>? Medias { get; set; } = null;
    public RoomStatusType? Status { get; set; }
    public List<Guid>? TaggedUserId { get; set; }
    public List<string>? HashTags { get; set; }
    public IEnumerable<RoomMemberModel> Members { get; set; } = [];
    public int OnlineCount {  get; set; }
}
