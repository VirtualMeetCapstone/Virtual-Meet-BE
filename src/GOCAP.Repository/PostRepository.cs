namespace GOCAP.Repository;

[RegisterService(typeof(IPostRepository))]
internal class PostRepository(AppSqlDbContext _context,
     IMapper _mapper) : SqlRepositoryBase<Post, PostEntity>(_context, _mapper), IPostRepository
{
}
