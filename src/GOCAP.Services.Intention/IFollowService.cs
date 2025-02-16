namespace GOCAP.Services.Intention;

public interface IFollowService : IServiceBase<Follow>
{
    Task<OperationResult> FollowOrUnfollowAsync(Follow follow);
}
