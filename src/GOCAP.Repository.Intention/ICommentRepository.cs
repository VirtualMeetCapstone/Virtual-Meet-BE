namespace GOCAP.Repository.Intention;

public interface ICommentRepository : IMongoRepositoryBase<CommentEntity>
{
    Task<QueryResult<CommentEntity>> GetByPostIdWithPagingAsync(Guid postId, QueryInfo queryInfo);
    Task<QueryResult<CommentEntity>> GetRepliesWithPagingAsync(Guid commentId, QueryInfo queryInfo);
}
