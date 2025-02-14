namespace GOCAP.Repository.Intention;

public interface IPostLikeRepository : ISqlRepositoryBase<PostReaction>
{
    Task<bool> CheckExistAsync(Guid postId, Guid userId);
    Task<int> DeleteAsync(Guid postId, Guid userId);
}
