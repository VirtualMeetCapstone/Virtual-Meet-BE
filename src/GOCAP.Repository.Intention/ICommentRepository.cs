namespace GOCAP.Repository.Intention;

public interface ICommentRepository : IMongoRepositoryBase<CommentEntity>
{
    Task<QueryResult<CommentEntity>> GetByPostId(Guid postId, QueryInfo queryInfo);
    Task<QueryResult<CommentEntity>> GetReplies(Guid commentId, QueryInfo queryInfo);
}
