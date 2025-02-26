namespace GOCAP.Domain;

public class MessageBuilder
{
    private readonly Message _message;
    public MessageBuilder()
    {
        _message = new Message();
    }
    public MessageBuilder SetType(MessageType type)
    {
        _message.Type = type;
        return this;
    }
    public MessageBuilder SetRoomId(Guid? roomId)
    {
        _message.RoomId = roomId;
        return this;
    }
    public MessageBuilder SetGroupId(Guid? groupId)
    {
        _message.GroupId = groupId;
        return this;
    }
    public MessageBuilder SetReceiverId(Guid? receiverId)
    {
        _message.ReceiverId = receiverId;
        return this;
    }
    public MessageBuilder SetSenderId(Guid senderId)
    {
        _message.SenderId = senderId;
        return this;
    }
    public MessageBuilder SetContent(string content)
    {
        _message.Content = content;
        return this;
    }
    public MessageBuilder AddAttachment(Media media)
    {
        _message.Attachments.Add(media);
        return this;
    }
    public Message Build()
    {
        if (string.IsNullOrEmpty(_message.Content) 
        && (_message.Attachments == null || _message.Attachments.Count == 0))
            throw new InvalidOperationException("Message must have either content or an attachment");

        return _message;
    }
}
