namespace GOCAP.Services.Intention;

public interface IPostService : IServiceBase<UserPost>
{
    Task<OperationResult> LikeOrUnlikeAsync(PostReaction postLike);
}
