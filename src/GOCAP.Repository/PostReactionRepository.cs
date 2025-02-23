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
             .AsNoTracking()
             .Where(r => postIds.Contains(r.PostId))
             .GroupBy(r => new { r.PostId, r.Type })
             .Select(g => new PostReactionCount
             {
                 PostId = g.Key.PostId,
                 Type = g.Key.Type ?? ReactionType.Default,
                 Count = g.Sum(r => 1)
             })
             .ToListAsync();
    }

    public async Task<QueryResult<UserReactionPost>> GetUserReactionsByPostIdAsync(Guid postId, QueryInfo queryInfo)
    {
        var reactions = await (from reaction in _context.PostReactions.AsNoTracking()
                               join user in _context.Users.AsNoTracking()
                               on reaction.UserId equals user.Id into userGroup
                               from user in userGroup.DefaultIfEmpty() 
                               where reaction.PostId == postId
                               orderby reaction.CreateTime descending
                               select new UserReactionPost
                               {
                                   Id = reaction.UserId,
                                   Name = user != null ? user.Name : "Unknown User",
                                   Media = JsonHelper.Deserialize<Media>(user.Picture),
                                   ReactionType = reaction.Type ?? ReactionType.Default
                               }).ToListAsync();

        int totalItems = queryInfo.NeedTotalCount
            ? await _context.PostReactions.Where(r => r.PostId == postId).CountAsync()
            : 0;

        return new QueryResult<UserReactionPost>
        {
            Data = reactions,
            TotalCount = totalItems
        };
    }
    public async Task<PostReactionEntity> GetByPostAndUserAsync(Guid postId, Guid userId)
    {
        return await _context.PostReactions
            .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId)
            ?? new PostReactionEntity();
    }
}
