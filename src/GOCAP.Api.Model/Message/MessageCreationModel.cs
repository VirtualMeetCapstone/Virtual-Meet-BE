namespace GOCAP.Api.Model;

public class MessageCreationModel
{
    public MessageType Type { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<Media>? Attachments { get; set; }
    public bool IsPinned { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? RoomId { get; set; }
    public Guid? GroupId { get; set; }
    public Guid? ReceiverId { get; set; }
}
