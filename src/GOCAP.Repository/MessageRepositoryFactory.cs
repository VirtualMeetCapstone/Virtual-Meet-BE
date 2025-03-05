namespace GOCAP.Repository;

[RegisterService(typeof(IMessageRepositoryFactory))]
internal class MessageRepositoryFactory(AppMongoDbContext _context) : IMessageRepositoryFactory
{
    public IMessageRepository<MessageEntity>? Create(MessageType type)
    {
        return type switch
        {
            MessageType.Direct => new MessageRepository<UserMessageEntity>(_context) as IMessageRepository<MessageEntity>,
            MessageType.Room => new MessageRepository<RoomMessageEntity>(_context) as IMessageRepository<MessageEntity>,
            MessageType.Group => new MessageRepository<GroupMessageEntity>(_context) as
              IMessageRepository<MessageEntity>,
            _ => throw new ParameterInvalidException($"Unsupported message type {nameof(type)}")
        };
    }
}
