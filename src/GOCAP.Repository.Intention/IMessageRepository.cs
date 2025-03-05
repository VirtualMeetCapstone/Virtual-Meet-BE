namespace GOCAP.Repository.Intention;

public interface IMessageRepository<T> : IMongoRepositoryBase<T> where T : MessageEntity
{
}
