namespace GOCAP.Repository;

[RegisterService(typeof(IMessageRepository<>))]
internal class MessageRepository<T>
    (AppMongoDbContext context)
    : MongoRepositoryBase<T>(context), IMessageRepository<T> where T : MessageEntity
{
}