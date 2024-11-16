namespace GOCAP.Services;

[RegisterService(typeof(IPostService))]
internal class PostService(
    IPostRepository _repository, 
    IMediaRepository _mediaRepository,
    ILogger<PostService> _logger
    ) : ServiceBase<Post>(_repository, _logger), IPostService
{
    public override async Task<IEnumerable<Post>> GetAllAsync()
    {
        var result = new List<Post>();
        var posts = await _repository.GetAllAsync();
        foreach (var post in posts)
        {
            result.Add(await GetFullPostInfo(post));
        }
        return result;
    }

    public override async Task<QueryResult<Post>> GetByPageAsync(QueryInfo queryInfo)
    {
        var posts = new List<Post>();
        var query = await _repository.GetByPageAsync(queryInfo);
        foreach (var post in query.Data)
        {
            posts.Add(await GetFullPostInfo(post));
        }
        return new QueryResult<Post>
        {
            Data = posts,
            TotalCount = posts.Count
        };
    }

    public override async Task<Post> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        result = await GetFullPostInfo(result);
        return result;
    }

    public async Task<Post> UploadPost(Post post)
    {
        return await _repository.AddAsync(post);
    }

    private async Task<Post> GetFullPostInfo(Post post)
    {
        post.Medias = await _mediaRepository.GetByPostIdAsync(post);
        return post;
    }
}
