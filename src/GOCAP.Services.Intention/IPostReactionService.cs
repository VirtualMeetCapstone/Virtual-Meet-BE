namespace GOCAP.Services.Intention;

public interface IPostReactionService
{
    Task<OperationResult> ReactOrUnreactedAsync(PostReaction postReaction);
    Task<QueryResult<UserReactionPost>> GetUserReactionsByPostIdAsync(Guid postId, QueryInfo queryInfo);
}
