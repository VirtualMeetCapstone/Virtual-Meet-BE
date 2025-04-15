namespace GOCAP.Services.Intention;

public interface IPostReactionService
{
    Task<OperationResult> ReactOrUnreactedAsync(PostReaction postReaction);
    Task<UserReactionPostQueryResult> GetUserReactionsByPostIdAsync(Guid postId, QueryInfo queryInfo);
}