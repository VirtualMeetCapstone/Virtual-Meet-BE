
namespace GOCAP.Domain;

public class Message : DateTrackingBase
{
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; }
    public List<Media> Attachments { get; set; } = [];
    public bool IsPinned { get; set; }
    public bool IsEdited { get; set; }
    public Guid ParentId { get; set; }
    public List<Guid> MentionedUserIds { get; set; } = [];
    public List<Guid> SeenBy { get; set; } = [];
    public Guid? RoomId { get; set; }
    public Guid? ReceiverId { get; set; }
    public Guid? GroupId { get; set; }
}
