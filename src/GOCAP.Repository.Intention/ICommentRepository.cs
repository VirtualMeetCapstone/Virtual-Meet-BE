namespace GOCAP.Repository.Intention;

public interface ICommentRepository : IMongoRepositoryBase<CommentEntity>
{
    Task<QueryResult<CommentEntity>> GetByPostIdWithPagingAsync(Guid postId, QueryInfo queryInfo);
    Task<QueryResult<CommentEntity>> GetRepliesAsyncWithPagingAsync(Guid commentId, QueryInfo queryInfo);
}
