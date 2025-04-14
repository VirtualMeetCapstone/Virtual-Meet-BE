
namespace GOCAP.Repository.Intention;

public interface IMessageRepository : IMongoRepositoryBase<MessageEntity>
{
    Task<QueryResult<Conversation>> GetMessagesByCurrentUserId(Guid currentUserId, QueryInfo queryInfo);
}
