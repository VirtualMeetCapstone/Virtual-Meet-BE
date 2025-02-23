using GOCAP.Repository.Intention;

namespace GOCAP.Services;

[RegisterService(typeof(IPostReactionService))]
internal class PostReactionService(
     IPostRepository _repository,
    IPostReactionRepository _postReactionRepository,
    IMapper _mapper,
    ILogger<PostService> _logger
    ) : ServiceBase<Post, PostEntity>(_repository, _mapper, _logger), IPostReactionService
{

    private readonly IMapper _mapper = _mapper;

    /// <summary>
    /// Like or unlike one post.
    /// </summary>
    /// <param name="postReaction"></param>
    /// <returns></returns>
    public async Task<OperationResult> ReactOrUnreactedAsync(PostReaction postReaction)
    {

        if (!Enum.IsDefined(typeof(ReactionType), postReaction.Type) || (int)postReaction.Type <= 0)
        {
            _logger.LogWarning("Invalid reaction type: {Type} for PostId: {PostId}, UserId: {UserId}",
                postReaction.Type, postReaction.PostId, postReaction.UserId);
            return new OperationResult(false, "Invalid reaction type.");
        }

        var existingReaction = await _postReactionRepository.GetByPostAndUserAsync(postReaction.PostId, postReaction.UserId);

        if (existingReaction.Type != null)
        {
            if (existingReaction.Type == postReaction.Type)
            {
                _logger.LogInformation("Start deleting reaction of type {Type}.", existingReaction.Type);
                var resultDelete = await _postReactionRepository.DeleteAsync(postReaction.PostId, postReaction.UserId);

                return new OperationResult(resultDelete > 0, resultDelete > 0 ? "Reaction removed." : "Failed to remove reaction.");
            }
            else
            {
                _logger.LogInformation("Updating reaction from {OldType} to {NewType}", existingReaction.Type, postReaction.Type);
                existingReaction.Type = postReaction.Type;
                await _postReactionRepository.UpdateAsync(existingReaction);
                return new OperationResult(true, "Reaction updated.");
            }
        }
        else
        {
            _logger.LogInformation("Adding new reaction: {Type} for PostId: {PostId}, UserId: {UserId}",
                postReaction.Type, postReaction.PostId, postReaction.UserId);

            postReaction.InitCreation();
            var entity = _mapper.Map<PostReactionEntity>(postReaction);
            await _postReactionRepository.AddAsync(entity);
            return new OperationResult(true, "Reaction added.");
        }
    }

    public async Task<QueryResult<UserReactionPost>> GetUserReactionsByPostIdAsync(Guid postId, QueryInfo queryInfo)
    {
        return await _postReactionRepository.GetUserReactionsByPostIdAsync(postId, queryInfo);
    }
}
