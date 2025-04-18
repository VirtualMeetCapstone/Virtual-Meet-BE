namespace GOCAP.Repository;

[RegisterService(typeof(IMessageReactionRepository))]
internal class MessageReactionRepository(AppMongoDbContext context)
    : MongoRepositoryBase<MessageReactionEntity>(context), IMessageReactionRepository
{
}
