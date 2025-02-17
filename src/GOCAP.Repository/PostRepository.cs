namespace GOCAP.Repository;

[RegisterService(typeof(IPostRepository))]
internal class PostRepository(AppSqlDbContext _context) : SqlRepositoryBase<PostEntity>(_context), IPostRepository
{
}
