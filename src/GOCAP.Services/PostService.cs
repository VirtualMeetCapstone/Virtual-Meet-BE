namespace GOCAP.Services;

[RegisterService(typeof(IPostService))]
internal class PostService(
    IPostRepository _repository,
    IPostReactionRepository _postReactionRepository,
    IUserRepository _userRepository,
    IBlobStorageService _blobStorageService,
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    ILogger<PostService> _logger
    ) : ServiceBase<Post, PostEntity>(_repository, _mapper, _logger), IPostService
{
    private readonly IMapper _mapper = _mapper;

    public async Task<Post> GetDetailByIdAsync(Guid id)
    {
        return await _repository.GetDetailByIdAsync(id);
    }

    /// <summary>
    /// Create a new Post
    /// </summary>
    /// <param name="Post"></param>
    /// <returns></returns>
    public override async Task<Post> AddAsync(Post post)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Post).Name);

        if (!await _userRepository.CheckExistAsync(post.UserId))
        {
            throw new ResourceNotFoundException($"User {post.UserId} was not found.");
        }

        // Upload file to azure.
        if (post.MediaUploads?.Count > 0)
        {
            var medias = await _blobStorageService.UploadFilesAsync(post.MediaUploads);
            post.Medias = medias;
        }

        post.InitCreation();
        try
        {
            var entity = _mapper.Map<PostEntity>(post);
            var PostDomain = await _repository.AddAsync(entity);
            return _mapper.Map<Post>(PostDomain);
        }
        catch (Exception ex)
        {
            if (post.MediaUploads != null && post.MediaUploads.Count > 0)
            {
                await MediaHelper.DeleteMediaFilesIfError(post.MediaUploads, _blobStorageService);
            }
            throw new InternalException(ex.Message);
        }
    }

    /// <summary>
    /// Delete a post by UserId.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public override async Task<OperationResult> DeleteByIdAsync(Guid id)
    {
        _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(Room).Name);
        try
        {
            // Begin transaction by unit of work to make sure the consistency
            await _unitOfWork.BeginTransactionAsync();

            // Post reaction
            await _postReactionRepository.DeleteByPostIdAsync(id);

            // Remove comments of the post.
            
            // Post
            var result = await _repository.DeleteByIdAsync(id);

            // Commit if success
            await _unitOfWork.CommitTransactionAsync();
            return new OperationResult(result > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting entity of type {EntityType}.", typeof(Room).Name);
            // Rollback if fail
            await _unitOfWork.RollbackTransactionAsync();
            return new OperationResult(false);
        }
    }

    /// <summary>
    /// Like or unlike one post.
    /// </summary>
    /// <param name="postReaction"></param>
    /// <returns></returns>
    public async Task<OperationResult> ReactOrUnreactAsync(PostReaction postReaction)
    {
        var result = new OperationResult(true);
        var isExists = await _postReactionRepository.CheckExistAsync(postReaction.PostId, postReaction.UserId);
        if (isExists)
        {
            _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(PostReaction).Name);
            var resultDelete = await _postReactionRepository.DeleteAsync(postReaction.PostId, postReaction.UserId);
            result = new OperationResult(resultDelete > 0);
        }
        else
        {
            _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(PostReaction).Name);
            postReaction.InitCreation();
            var entity = _mapper.Map<PostReactionEntity>(postReaction);
            await _postReactionRepository.AddAsync(entity);
        }
        return result;
    }
}
