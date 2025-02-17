namespace GOCAP.Repository.Intention;

public interface IFollowRepository : ISqlRepositoryBase<UserFollowEntity>
{
    Task<UserFollowEntity?> GetByFollowerAndFollowingAsync(Guid followerId, Guid followingId);
}
