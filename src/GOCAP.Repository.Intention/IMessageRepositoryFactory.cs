namespace GOCAP.Repository.Intention;

public interface IMessageRepositoryFactory
{
    IMessageRepository<MessageEntity>? Create(MessageType type);
}
