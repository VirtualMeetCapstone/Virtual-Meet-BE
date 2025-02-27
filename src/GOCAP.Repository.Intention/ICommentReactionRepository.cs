namespace GOCAP.Repository.Intention;
public interface ICommentReactionRepository : IMongoRepositoryBase<CommentReactionEntity>
{
    Task<CommentReactionEntity?> GetByCommentAndUserAsync(Guid commentId, Guid userId);
    Task<int> DeleteAsync(Guid commentId, Guid userId);
}
