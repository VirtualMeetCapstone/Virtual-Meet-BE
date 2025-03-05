namespace GOCAP.Api.Model;

public class MessageModel : DateTrackingBase
{
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; }
    public List<Media> Attachments { get; set; } = [];
    public bool IsPinned { get; set; }
    public Guid ParentId { get; set; }
}
