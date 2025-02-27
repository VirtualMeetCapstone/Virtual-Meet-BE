namespace GOCAP.Repository;

[RegisterService(typeof(ICommentReactionRepository))]
internal class CommentReactionRepository(AppMongoDbContext context)
    : MongoRepositoryBase<CommentReactionEntity>(context), ICommentReactionRepository
{
    private readonly AppMongoDbContext _context = context;
    public async Task<CommentReactionEntity?> GetByCommentAndUserAsync(Guid commentId, Guid userId)
    {
        return await _context.CommentReactions
            .Find(r => r.CommentId == commentId && r.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> DeleteAsync(Guid commentId, Guid userId)
    {
        var result = await _context.CommentReactions
            .DeleteOneAsync(r => r.CommentId == commentId && r.UserId == userId);
        return (int)result.DeletedCount;
    }

}
