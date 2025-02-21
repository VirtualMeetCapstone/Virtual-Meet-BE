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
            ?? throw new ResourceNotFoundException("This reaction does not exists.");
        _context.PostReactions.Remove(postLike);
        return await _context.SaveChangesAsync();
    }
    public async Task<OperationResult> DeleteByPostIdAsync(Guid id)
    {
        var reactions = await _context.PostReactions
                               .Where(r => r.PostId == id)
                               .ToListAsync();

        if (reactions.Count == 0)
        {
            return new OperationResult(false, "No reactions found for this post.");
        }

        _context.PostReactions.RemoveRange(reactions);
        await _context.SaveChangesAsync();

        return new OperationResult(true, "Delete OK");
    }

    public async Task<List<PostReactionCount>> GetReactionsByPostIdsAsync(List<Guid> postIds)
    {
        return await _context.PostReactions
             .Where(r => postIds.Contains(r.PostId))
             .GroupBy(r => new { r.PostId, r.Type })
             .Select(g => new PostReactionCount
             {
                 PostId = g.Key.PostId,
                 Type = (ReactionType)g.Key.Type!,
                 Count = g.Count()
             })
             .ToListAsync();
    }
}
