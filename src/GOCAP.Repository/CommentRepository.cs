namespace GOCAP.Repository;

[RegisterService(typeof(ICommentRepository))]
internal class CommentRepository(AppMongoDbContext context)
    : MongoRepositoryBase<CommentEntity>(context), ICommentRepository
{
    private readonly AppMongoDbContext _context = context;
    public async Task<QueryResult<CommentEntity>> GetByPostIdWithPagingAsync(Guid postId, QueryInfo queryInfo)
    {
        var filter = Builders<CommentEntity>.Filter.Eq(c => c.PostId, postId);

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
                                              .ToListAsync();

        var replyCountDict = comments
          .Where(c => c.ParentId.HasValue && c.ParentId != Guid.Empty)
          .GroupBy(c => c.ParentId!.Value)
          .ToDictionary(g => g.Key, g => g.Count());

        var updatedComments = comments.Select(c => new CommentEntity
        {
            Id = c.Id,
            PostId = c.PostId,
            Author = c.Author,
            Content = c.Content,
            Mentions = c.Mentions,
            ParentId = c.ParentId,
            CreateTime = c.CreateTime,
            LastModifyTime = c.LastModifyTime,
            ReplyCount = replyCountDict.TryGetValue(c.Id, out var count) ? count : 0
        }).ToList();

        return new QueryResult<CommentEntity>
        {
            Data = updatedComments,
            TotalCount = (int)totalComments
        };
    }

    public async Task<QueryResult<CommentEntity>> GetRepliesAsyncWithPagingAsync(Guid commentId, QueryInfo queryInfo)
    {
        var filter = Builders<CommentEntity>.Filter.Eq(c => c.ParentId, commentId);

        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            var textFilter = Builders<CommentEntity>.Filter.Regex(c => c.Content, new BsonRegularExpression(queryInfo.SearchText, "i"));
            filter = Builders<CommentEntity>.Filter.And(filter, textFilter);
        }
     
        var orderByField = !string.IsNullOrEmpty(queryInfo.OrderBy) ? queryInfo.OrderBy : nameof(CommentEntity.CreateTime);
        var sortDefinition = queryInfo.OrderType == OrderType.Ascending
            ? Builders<CommentEntity>.Sort.Ascending(orderByField)
            : Builders<CommentEntity>.Sort.Descending(orderByField);

        long totalComments = queryInfo.NeedTotalCount ? await _context.Comments.CountDocumentsAsync(filter) : 0;

        var updatedComments = await _context.Comments
            .Find(filter)
            .Sort(sortDefinition)
            .Skip(queryInfo.Skip)
            .Limit(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<CommentEntity>
        {
            Data = updatedComments,
            TotalCount = (int)totalComments
        };
    }

}
