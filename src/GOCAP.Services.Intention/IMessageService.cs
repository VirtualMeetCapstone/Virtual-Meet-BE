
namespace GOCAP.Services.Intention;

public interface IMessageService : IServiceBase<Message>
{
    Task<QueryResult<Conversation>> GetConversations(QueryInfo queryInfo);
}
