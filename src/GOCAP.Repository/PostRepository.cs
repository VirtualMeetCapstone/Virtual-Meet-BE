namespace GOCAP.Repository;

[RegisterService(typeof(IPostRepository))]
internal class PostRepository(AppSqlDbContext _context, IMapper _mapper) : SqlRepositoryBase<PostEntity>(_context),
    IPostRepository
{
    private readonly IMapper _mapper = _mapper;
    private readonly AppSqlDbContext _context = _context;
    public async Task<QueryResult<Post>> GetWithPagingAsync(QueryInfo queryInfo)
    {
        var query = _context.Posts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            query = query.Where(r => EF.Functions.Collate(r.Content, "Latin1_General_CI_AI").Contains(queryInfo.SearchText.Trim()));
        }

        var totalItems = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        var postsResult = await query
            .OrderByDescending(r => r.CreateTime)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(post => new
            {
                PostEntity = post,
                UserName = post.User != null ? post.User.Name : "Unknown",
                Photo = post.User != null ? post.User.Picture : null
            })
            .ToListAsync();

        if (postsResult.Count == 0)
        {
            return new QueryResult<Post>
            {
                Data = [],
                TotalCount = totalItems
            };
        }

        var postIds = postsResult.Select(p => p.PostEntity.Id).ToList();

        var reactionsRaw = await _context.PostReactions
             .AsNoTracking()
            .Where(r => postIds.Contains(r.PostId))
            .ToListAsync();

        var reactionsDict = reactionsRaw
            .GroupBy(r => r.PostId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Total = g.Count(),
                    TypeCounts = g.Where(r => r.Type.HasValue)
                                  .GroupBy(r => (int)r.Type!)
                                  .ToDictionary(gr => gr.Key, gr => gr.Count())
                }
            );

        var postDomain = postsResult.Select(p =>
        {
            var domainPost = _mapper.Map<Post>(p.PostEntity);
            domainPost.User = new User
            {
                Name = p.UserName,
                Picture = JsonHelper.Deserialize<Media>(p.Photo)
            };

            if (reactionsDict.TryGetValue(domainPost.Id, out var postReactions))
            {
                domainPost.TotalReactions = postReactions.Total;
                domainPost.ReactionCounts = postReactions.TypeCounts;
            }
            else
            {
                domainPost.TotalReactions = 0;
                domainPost.ReactionCounts = [];
            }

            return domainPost;
        }).ToList();

        return new QueryResult<Post>
        {
            Data = postDomain,
            TotalCount = totalItems
        };
    }

    public async Task<Post> GetDetailByIdAsync(Guid id)
    {
        var postWithReactions = await _context.Posts
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(post => new
            {
                PostEntity = post,
                UserName = post.User != null ? post.User.Name : "Unknown",
                Photo = post.User != null ? post.User.Picture : null
            })
            .FirstOrDefaultAsync()
            ?? throw new ResourceNotFoundException($"Entity with ID {id} was not found.");

        var reactionsRaw = await _context.PostReactions
             .AsNoTracking()
            .Where(r => r.PostId == id)
            .ToListAsync();

        var reactionsData = reactionsRaw
            .GroupBy(r => r.PostId)
            .Select(g => new
            {
                Total = g.Count(),
                TypeCounts = g.Where(r => r.Type.HasValue)
                              .GroupBy(r => (int)r.Type!)
                              .ToDictionary(gr => gr.Key, gr => gr.Count())
            })
            .FirstOrDefault() ?? new { Total = 0, TypeCounts = new Dictionary<int, int>() };

        var post = _mapper.Map<Post>(postWithReactions.PostEntity);
        post.User = new User
        {
            Name = postWithReactions.UserName,
            Picture = JsonHelper.Deserialize<Media>(postWithReactions.Photo)
        };
        post.TotalReactions = reactionsData.Total;
        post.ReactionCounts = reactionsData.TypeCounts;

        return post;
    }

    //Get posts created by userID
    public async Task<QueryResult<Post>> GetPostByUserIdAsync(Guid userId, QueryInfo queryInfo)
    {
        var postEntities = await _context.Posts
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(post => new
            {
                PostEntity = post,
                UserName = post.User != null ? post.User.Name : "Unknown",
                Photo = post.User != null ? post.User.Picture : null
            })
            .ToListAsync();

        var postIds = postEntities.Select(p => p.PostEntity.Id).ToList();

        var reactionsRaw = await _context.PostReactions
            .AsNoTracking()
            .Where(r => postIds.Contains(r.PostId))
            .ToListAsync();

        var reactionsDict = reactionsRaw
            .GroupBy(r => r.PostId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Total = g.Count(),
                    TypeCounts = g.Where(r => r.Type.HasValue)
                                  .GroupBy(r => (int)r.Type!)
                                  .ToDictionary(gr => gr.Key, gr => gr.Count())
                }
            );

        var data = postEntities.Select(p =>
        {
            var post = _mapper.Map<Post>(p.PostEntity);
            post.User = new User
            {
                Name = p.UserName,
                Picture = JsonHelper.Deserialize<Media>(p.Photo)
            };

            if (reactionsDict.TryGetValue(post.Id, out var postReactions))
            {
                post.TotalReactions = postReactions.Total;
                post.ReactionCounts = postReactions.TypeCounts;
            }
            else
            {
                post.TotalReactions = 0;
                post.ReactionCounts = [];
            }

            return post;
        }).ToList();

        int totalItems = queryInfo.NeedTotalCount
            ? await _context.Posts.CountAsync()
            : 0;

        return new QueryResult<Post>
        {
            Data = data,
            TotalCount = totalItems
        };
    }

    //Get posts where userID reacted
    public async Task<QueryResult<Post>> GetPostsUserReactedAsync(Guid userId, QueryInfo queryInfo)
    {
        var postEntities = await _context.Posts
            .AsNoTracking()
            .Where(p => p.Reactions.Any(r => r.UserId == userId))
            .Select(post => new
            {
                PostEntity = post,
                UserName = post.User != null ? post.User.Name : "Unknown",
                Photo = post.User != null ? post.User.Picture : null
            })
            .ToListAsync();

        var postIds = postEntities.Select(p => p.PostEntity.Id).ToList();

        var reactionsRaw = await _context.PostReactions
            .AsNoTracking()
            .Where(r => postIds.Contains(r.PostId))
            .ToListAsync();

        var reactionsDict = reactionsRaw
            .GroupBy(r => r.PostId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Total = g.Count(),
                    TypeCounts = g.Where(r => r.Type.HasValue)
                                  .GroupBy(r => (int)r.Type!)
                                  .ToDictionary(gr => gr.Key, gr => gr.Count())
                }
            );

        var data = postEntities.Select(p =>
        {
            var post = _mapper.Map<Post>(p.PostEntity);
            post.User = new User
            {
                Name = p.UserName,
                Picture = JsonHelper.Deserialize<Media>(p.Photo)
            };

            if (reactionsDict.TryGetValue(post.Id, out var postReactions))
            {
                post.TotalReactions = postReactions.Total;
                post.ReactionCounts = postReactions.TypeCounts;
            }
            else
            {
                post.TotalReactions = 0;
                post.ReactionCounts = [];
            }

            return post;
        }).ToList();

        int totalItems = queryInfo.NeedTotalCount
            ? await _context.Posts.CountAsync()
            : 0;

        return new QueryResult<Post>
        {
            Data = data,
            TotalCount = totalItems
        };
    }
}
