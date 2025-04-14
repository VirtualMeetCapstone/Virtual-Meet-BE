namespace GOCAP.Repository;

[RegisterService(typeof(IMessageRepository))]
internal class MessageRepository(AppMongoDbContext context)
    : MongoRepositoryBase<MessageEntity>(context), IMessageRepository
{
    public Task<QueryResult<Conversation>> GetMessagesByCurrentUserId(Guid currentUserId, QueryInfo queryInfo)
    {
        throw new NotImplementedException();
    }
}