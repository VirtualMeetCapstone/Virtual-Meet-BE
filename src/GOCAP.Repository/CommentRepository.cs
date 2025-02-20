
namespace GOCAP.Repository;

[RegisterService(typeof(ICommentRepository))]
internal class CommentRepository(AppMongoDbContext context)
    : MongoRepositoryBase<CommentEntity>(context), ICommentRepository
{
    private readonly AppMongoDbContext _context = context;
    public async Task<QueryResult<CommentEntity>> GetByPostIdWithPagingAsync(Guid postId, QueryInfo queryInfo)
    {
        var filter = Builders<CommentEntity>.Filter.Eq(c => c.PostId, postId);

        // Add condition if having search text.
        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            var textFilter = Builders<CommentEntity>.Filter.Regex(c => c.Content, new BsonRegularExpression(queryInfo.SearchText, "i"));
            filter = Builders<CommentEntity>.Filter.And(filter, textFilter);
        }

        var orderByField = !string.IsNullOrEmpty(queryInfo.OrderBy) ? queryInfo.OrderBy : AppConstants.DefaultOrderBy;
        var sortDefinition = queryInfo.OrderType == OrderType.Ascending
            ? Builders<CommentEntity>.Sort.Ascending(orderByField)
            : Builders<CommentEntity>.Sort.Descending(orderByField);
        long totalComments = queryInfo.NeedTotalCount ? await _context.Comments.CountDocumentsAsync(filter) : 0;

        var comments = await _context.Comments.Find(filter)
                                              .Sort(sortDefinition)
                                              .Skip(queryInfo.Skip)
                                              .Limit(queryInfo.Top)
                                              .ToListAsync();
        return new QueryResult<CommentEntity>
        {
            Data = comments,
            TotalCount = (int)totalComments
        };
    }
}
