namespace GOCAP.Repository;

[RegisterService(typeof(IMessageRepository))]
internal class MessageRepository
    (AppMongoDbContext context)
    : MongoRepositoryBase<MessageEntity>(context), IMessageRepository
{
}