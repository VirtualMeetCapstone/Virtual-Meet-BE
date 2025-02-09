namespace GOCAP.Repository.Intention;

public interface ICommentRepository : IMongoRepositoryBase<Comment>
{
    Task<QueryResult<Comment>> GetByPostIdWithPagingAsync(Guid postId, QueryInfo queryInfo);
}
