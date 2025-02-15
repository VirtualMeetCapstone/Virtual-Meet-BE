namespace GOCAP.Services;

[RegisterService(typeof(IPostService))]
internal class PostService(
    IPostRepository _repository,
    IPostLikeRepository _postLikeRepository,
    ILogger<PostService> _logger
    ) : ServiceBase<Post>(_repository, _logger), IPostService
{
    /// <summary>
    /// Like or unlike one post.
    /// </summary>
    /// <param name="postLike"></param>
    /// <returns></returns>
    public async Task<OperationResult> LikeOrUnlikeAsync(PostReaction postLike)
    {
        var result = new OperationResult(true);
        var isExists = await _postLikeRepository.CheckExistAsync(postLike.PostId, postLike.UserId);
        if (isExists)
        {
            _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(PostReaction).Name);
            var resultDelete = await _postLikeRepository.DeleteAsync(postLike.PostId, postLike.UserId);
            result = new OperationResult(resultDelete > 0);
        }
        else
        {
            _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(PostReaction).Name);
            postLike.InitCreation();
            await _postLikeRepository.AddAsync(postLike);
        }
        return result;
    }
}
