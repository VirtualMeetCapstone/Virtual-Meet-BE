namespace GOCAP.Services.Intention;

public interface IFollowService : IServiceBase<Follow>
{
    Task<OperationResult> FollowOrUnfollowAsync(Follow domain);
    Task<bool> IsFollowingAsync(Guid followingId);
}
