namespace GOCAP.Api.Model;

public interface IMessageCreationModel
{
    Guid SenderId { get; set; }
    string Content { get; set; }
    List<Media>? Attachments { get; set; }
    bool IsPinned { get; set; }
    Guid? ParentId { get; set; }
}