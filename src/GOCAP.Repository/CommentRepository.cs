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

        var commentsResult = await _context.Comments.Find(filter).Sort(sortDefinition).ToListAsync();
        var commentIds = commentsResult.Select(c => c.Id).ToList();

        var replyCountDict = commentsResult
            .Where(c => c.ParentId.HasValue && c.ParentId != Guid.Empty)
            .GroupBy(c => c.ParentId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var reactionsRaw = await _context.CommentReactions.Find(r => commentIds.Contains(r.CommentId)).ToListAsync();
        var reactionsDict = reactionsRaw.GroupBy(r => r.CommentId).ToDictionary(
            g => g.Key,
            g => new
            {
                Total = g.Count(),
                TypeCounts = g.Where(r => r.Type.HasValue)
                              .GroupBy(r => (int)r.Type!)
                              .ToDictionary(gr => gr.Key, gr => gr.Count())
            }
        );

        var updatedComments = commentsResult.Select(c => new CommentEntity
        {
            Id = c.Id,
            PostId = c.PostId,
            Author = c.Author,
            Content = c.Content,
            Mentions = c.Mentions,
            ParentId = c.ParentId,
            CreateTime = c.CreateTime,
            LastModifyTime = c.LastModifyTime,
            ReplyCount = replyCountDict.TryGetValue(c.Id, out var count) ? count : 0,
            TotalReactions = reactionsDict.TryGetValue(c.Id, out var commentReactions) ? commentReactions.Total : 0,
            ReactionCounts = reactionsDict.TryGetValue(c.Id, out commentReactions) ? commentReactions.TypeCounts : []
        }).ToList();

        return new QueryResult<CommentEntity>
        {
            Data = updatedComments,
            TotalCount = (int)totalComments
        };
    }
    public async Task<QueryResult<CommentEntity>> GetRepliesWithPagingAsync(Guid commentId, QueryInfo queryInfo)
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

        var commentsResult = await _context.Comments
            .Find(filter)
            .Sort(sortDefinition)
            .Skip(queryInfo.Skip)
            .Limit(queryInfo.Top)
            .ToListAsync();

        var commentIds = commentsResult.Select(c => c.Id).ToList();

        var replyCountDict = await _context.Comments
            .Find(Builders<CommentEntity>.Filter.In(nameof(CommentEntity.ParentId), commentIds))
            .ToListAsync();

        var replyCounts = replyCountDict.GroupBy(c => c.ParentId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var reactionsRaw = await _context.CommentReactions.Find(r => commentIds.Contains(r.CommentId)).ToListAsync();
        var reactionsDict = reactionsRaw.GroupBy(r => r.CommentId).ToDictionary(
            g => g.Key,
            g => new
            {
                Total = g.Count(),
                TypeCounts = g.Where(r => r.Type.HasValue)
                              .GroupBy(r => (int)r.Type!)
                              .ToDictionary(gr => gr.Key, gr => gr.Count())
            }
        );

        var updatedComments = commentsResult.Select(c => new CommentEntity
        {
            Id = c.Id,
            PostId = c.PostId,
            Author = c.Author,
            Content = c.Content,
            Mentions = c.Mentions,
            ParentId = c.ParentId,
            CreateTime = c.CreateTime,
            LastModifyTime = c.LastModifyTime,
            ReplyCount = replyCounts.TryGetValue(c.Id, out var count) ? count : 0,
            TotalReactions = reactionsDict.TryGetValue(c.Id, out var commentReactions) ? commentReactions.Total : 0,
            ReactionCounts = reactionsDict.TryGetValue(c.Id, out commentReactions) ? commentReactions.TypeCounts : []
        }).ToList();

        return new QueryResult<CommentEntity>
        {
            Data = updatedComments,
            TotalCount = (int)totalComments
        };
    }
    public async Task<Dictionary<Guid, int>> GetCommentCountsByPostIdsAsync(List<Guid> postIds)
    {
        var filter = Builders<CommentEntity>.Filter.And(
            Builders<CommentEntity>.Filter.In(c => c.PostId, postIds),
            Builders<CommentEntity>.Filter.Or(
                Builders<CommentEntity>.Filter.Eq(c => c.ParentId, null),
                Builders<CommentEntity>.Filter.Eq(c => c.ParentId, Guid.Empty)
            )
        );

        var commentCounts = await _context.Comments
            .Aggregate()
            .Match(filter)
            .Group(c => c.PostId, g => new { PostId = g.Key, Count = g.Count() })
            .ToListAsync();

        return commentCounts.ToDictionary(x => x.PostId, x => x.Count);
    }


}