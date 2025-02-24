namespace GOCAP.Repository;

[RegisterService(typeof(ICommentReplyRepository))]
internal class CommentReplyRepository (AppMongoDbContext context)
    : MongoRepositoryBase<CommentReplyEntity>(context), ICommentReplyRepository
{
    private readonly AppMongoDbContext _context = context;

    public async Task<QueryResult<CommentReplyEntity>> GetByParentIdAsync(Guid parentId, QueryInfo queryInfo)
    {
        var filter = Builders<CommentReplyEntity>.Filter.Eq(r => r.ParentId, parentId);

        // Add condition if having search text.
        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            var textFilter = Builders<CommentReplyEntity>.Filter.Regex(c => c.Content, new BsonRegularExpression(queryInfo.SearchText, "i"));
            filter = Builders<CommentReplyEntity>.Filter.And(filter, textFilter);
        }

        var orderByField = !string.IsNullOrEmpty(queryInfo.OrderBy) ? queryInfo.OrderBy : AppConstants.DefaultOrderBy;
        var sortDefinition = queryInfo.OrderType == OrderType.Ascending
            ? Builders<CommentReplyEntity>.Sort.Ascending(orderByField)
            : Builders<CommentReplyEntity>.Sort.Descending(orderByField);
        long totalCount = queryInfo.NeedTotalCount ? await _context.CommentReplies.CountDocumentsAsync(filter) : 0;

        var items = await _context.CommentReplies
            .Find(filter)
            .Sort(sortDefinition) 
            .Skip(queryInfo.Skip)
            .Limit(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<CommentReplyEntity>
        {
            Data = items,
            TotalCount = (int)totalCount
        };
    }
}
