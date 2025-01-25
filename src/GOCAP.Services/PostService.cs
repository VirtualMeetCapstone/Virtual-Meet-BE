namespace GOCAP.Services;

[RegisterService(typeof(IPostService))]
internal class PostService(
    IPostRepository _repository, 
    ILogger<PostService> _logger
    ) : ServiceBase<UserPost>(_repository, _logger), IPostService
{

    public async Task<UserPost> UploadPost(UserPost post)
    {
        return await _repository.AddAsync(post);
    }
}
