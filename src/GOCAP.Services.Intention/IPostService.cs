namespace GOCAP.Services.Intention;

public interface IPostService : IServiceBase<Post>
{
    Task<OperationResult> LikeOrUnlikeAsync(PostReaction postLike);
}
