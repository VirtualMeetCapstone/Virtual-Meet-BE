namespace GOCAP.Repository;

[RegisterService(typeof(ICommentRepository))]
internal class CommentRepository(GoCapMongoDbContext _context,
    ILogger<MongoRepositoryBase<Comment>> _logger) : MongoRepositoryBase<Comment>(_context, _logger), ICommentRepository
{
}
