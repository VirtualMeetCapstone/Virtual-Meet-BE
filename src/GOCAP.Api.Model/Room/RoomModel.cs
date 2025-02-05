namespace GOCAP.Api.Model;

public class RoomModel
{
    public Guid Id { get; set; }
    public Guid? OwnerId { get; set; }
    public RoomMemberModel? Owner { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MaximumMembers { get; set; }
    public string? Medias { get; set; }
    public RoomStatusType? Status { get; set; }
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
    public IEnumerable<RoomMemberModel> Members { get; set; } = [];
}
