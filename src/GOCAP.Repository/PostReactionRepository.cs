namespace GOCAP.Repository;

[RegisterService(typeof(IPostReactionRepository))]
internal class PostReactionRepository(AppSqlDbContext context) :
    SqlRepositoryBase<PostReactionEntity>(context), IPostReactionRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<bool> CheckExistAsync(Guid postId, Guid userId)
    => await _context.PostReactions.AnyAsync(rf => rf.PostId == postId
                                                && rf.UserId == userId);

    public async Task<int> DeleteAsync(Guid postId, Guid userId)
    {
        var postLike = await _context.PostReactions
            .FirstOrDefaultAsync(rf => rf.PostId == postId && rf.UserId == userId)
            ?? throw new ResourceNotFoundException("This like does not exists.");
        _context.PostReactions.Remove(postLike);
        return await _context.SaveChangesAsync();
    }
}
