namespace GOCAP.Api.Model;

public abstract class MessageBaseModel
{
    public MessageType Type { get; set; }
    public Guid? RoomId { get; set; }
    public Guid? GroupId { get; set; }
    public Guid? ReceiverId { get; set; }
}
