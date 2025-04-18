namespace GOCAP.Repository.Intention;

public interface IPostReactionRepository : ISqlRepositoryBase<PostReactionEntity>
{
    Task<bool> CheckExistAsync(Guid postId, Guid userId);
    Task<int> DeleteAsync(Guid postId, Guid userId);
    Task<OperationResult> DeleteByPostIdAsync(Guid id);
    Task<List<PostReactionCount>> GetReactionsByPostIdsAsync(List<Guid> postIds);
    Task<PostReactionEntity> GetByPostAndUserAsync(Guid postId, Guid userId);
    Task<UserReactionPostQueryResult> GetUserReactionsByPostIdAsync(Guid postId, QueryInfo queryInfo);
}
