namespace GOCAP.Repository;

[RegisterService(typeof(IPostLikeRepository))]
internal class PostLikeRepository(AppSqlDbContext context, IMapper mapper) :
    SqlRepositoryBase<PostLike, UserPostLikeEntity>(context, mapper), IPostLikeRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<bool> CheckExistAsync(Guid postId, Guid userId)
    => await _context.UserPostLikes.AnyAsync(rf => rf.PostId == postId
                                                && rf.UserId == userId);

    public async Task<int> DeleteAsync(Guid postId, Guid userId)
    {
        var postLike = await _context.UserPostLikes
            .FirstOrDefaultAsync(rf => rf.PostId == postId && rf.UserId == userId)
            ?? throw new ResourceNotFoundException("This like does not exists.");
        _context.UserPostLikes.Remove(postLike);
        return await _context.SaveChangesAsync();
    }
}
