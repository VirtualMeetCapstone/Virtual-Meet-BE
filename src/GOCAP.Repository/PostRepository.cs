namespace GOCAP.Repository;

[RegisterService(typeof(IPostRepository))]
internal class PostRepository(AppSqlDbContext _context, IMapper _mapper) : SqlRepositoryBase<PostEntity>(_context),
    IPostRepository
{
    private readonly IMapper _mapper = _mapper;
    private readonly AppSqlDbContext _context = _context;
    public async Task<Post> GetDetailByIdAsync(Guid id)
    {
        var postEntity = await _context.Posts
            .FirstOrDefaultAsync(p => p.Id == id) 
            ?? throw new ResourceNotFoundException($"Entity with {id} was not found."); 

        var domain = _mapper.Map<Post>(postEntity);
        domain.CountReaction = await _context.PostReactions.CountAsync(p => p.PostId == id);

        return domain;
    }
}
