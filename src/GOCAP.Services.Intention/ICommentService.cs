namespace GOCAP.Services.Intention;
public interface ICommentService : IServiceBase<Comment>
{
    Task<QueryResult<Comment>> GetByPostIdWithPagingAsync(Guid postId, QueryInfo queryInfo);
    Task<QueryResult<Comment>> GetRepliesAsyncWithPagingAsync(Guid commentId, QueryInfo queryInfo);
    Task<OperationResult> ReactOrUnreactedAsync(CommentReaction commentReaction);
}
