namespace GOCAP.Api.Model;

public class MessageCreationModel : MessageBaseModel
{
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<Media>? Attachments { get; set; }
    public bool IsPinned { get; set; }
    public Guid? ParentId { get; set; }
}
