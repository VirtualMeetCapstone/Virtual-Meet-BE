namespace GOCAP.Services.Intention;

public interface IPostService : IServiceBase<Post>
{
    Task<OperationResult> ReactOrUnreactAsync(PostReaction postReaction);
}
