namespace GOCAP.Services;

[RegisterService(typeof(IPostService))]
internal class PostService(
    IPostRepository _repository, 
    ILogger<PostService> _logger
    ) : ServiceBase<Post>(_repository, _logger), IPostService
{

    public async Task<Post> UploadPost(Post post)
    {
        return await _repository.AddAsync(post);
    }
}
