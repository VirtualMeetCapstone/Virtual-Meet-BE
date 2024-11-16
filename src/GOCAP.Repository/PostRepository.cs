using AutoMapper;

namespace GOCAP.Repository;

[RegisterService(typeof(IPostRepository))]
internal class PostRepository(GoCapMsSqlDbContext _context,
     IMapper _mapper) : RepositoryBase<Post, PostEntity>(_context, _mapper), IPostRepository
{
}
