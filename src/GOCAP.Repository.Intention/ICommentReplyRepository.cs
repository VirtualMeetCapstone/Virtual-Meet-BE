namespace GOCAP.Repository.Intention;

public interface ICommentReplyRepository : IMongoRepositoryBase<CommentReplyEntity>
{
    Task<QueryResult<CommentReplyEntity>> GetByParentIdAsync(Guid parentId, QueryInfo queryInfo);
}
