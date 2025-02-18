namespace GOCAP.Services;

[RegisterService(typeof(IPostService))]
internal class PostService(
    IPostRepository _repository,
    IPostLikeRepository _postLikeRepository,
    IMapper _mapper,
    ILogger<PostService> _logger
    ) : ServiceBase<Post, PostEntity>(_repository, _mapper, _logger), IPostService
{
    private readonly IMapper _mapper = _mapper;
    /// <summary>
    /// Like or unlike one post.
    /// </summary>
    /// <param name="postReaction"></param>
    /// <returns></returns>
    public async Task<OperationResult> ReactOrUnreactAsync(PostReaction postReaction)
    {
        var result = new OperationResult(true);
        var isExists = await _postLikeRepository.CheckExistAsync(postReaction.PostId, postReaction.UserId);
        if (isExists)
        {
            _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(PostReaction).Name);
            var resultDelete = await _postLikeRepository.DeleteAsync(postReaction.PostId, postReaction.UserId);
            result = new OperationResult(resultDelete > 0);
        }
        else
        {
            _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(PostReaction).Name);
            postReaction.InitCreation();
            var entity = _mapper.Map<PostReactionEntity>(postReaction);
            await _postLikeRepository.AddAsync(entity);
        }
        return result;
    }
}
