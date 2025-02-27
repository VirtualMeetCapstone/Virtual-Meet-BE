namespace GOCAP.Services;

[RegisterService(typeof(ICommentService))]
internal class CommentService(
    ICommentRepository _repository,
    IPostRepository _postRepository,
    IUserRepository _userRepository,
    ICommentReactionRepository _commentReactionRepository,
    IMapper _mapper,
    ILogger<CommentService> _logger
    ) : ServiceBase<Comment, CommentEntity>(_repository, _mapper, _logger), ICommentService
{
    private readonly IMapper _mapper = _mapper;
    public override async Task<Comment> AddAsync(Comment domain)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Comment).Name);

        if (domain.Author == null)
        {
            throw new ParameterInvalidException();
        }

        domain.InitCreation();

        // Check whether post id and author existed or not.
        var isPostExists = await _postRepository.CheckExistAsync(domain.PostId);
        var author = await _userRepository.GetByIdAsync(domain.Author.Id);

        if (!isPostExists)
        {
            throw new ResourceNotFoundException($"Post {domain.PostId} does not found.");
        }

        if (domain.ParentId.HasValue && domain.ParentId.Value != Guid.Empty)
        {
            var parentCommentExists = await _repository.CheckExistAsync(domain.ParentId.Value);
            if (!parentCommentExists)
            {
                throw new ResourceNotFoundException($"Parent comment {domain.ParentId.Value} does not exist.");
            }
        }

        domain.Author = new CommentAuthor
        {
            Id = author.Id,
            Name = author.Name,
            Picture = author.Picture
        };

        var entity = _mapper.Map<CommentEntity>(domain);
        var result = await _repository.AddAsync(entity);
        return _mapper.Map<Comment>(result);
    }
    public async Task<QueryResult<Comment>> GetByPostId(Guid postId, QueryInfo queryInfo)
    {
        var result = await _repository.GetByPostId(postId, queryInfo);
        return _mapper.Map<QueryResult<Comment>>(result);
    }
    public async Task<QueryResult<Comment>> GetReplies(Guid commentId, QueryInfo queryInfo)
    {
        var result = await _repository.GetReplies(commentId, queryInfo);
        return _mapper.Map<QueryResult<Comment>>(result);
    }
    public async Task<OperationResult> ReactOrUnreactedAsync(CommentReaction commentReaction)
    {
        if (!Enum.IsDefined(typeof(ReactionType), commentReaction.Type) || (int)commentReaction.Type <= 0)
        {
            _logger.LogWarning("Invalid reaction type: {Type} for CommentId: {CommentId}, UserId: {UserId}",
                commentReaction.Type, commentReaction.CommentId, commentReaction.UserId);
            return new OperationResult(false, "Invalid reaction type.");
        }

        var existingReaction = await _commentReactionRepository.GetByCommentAndUserAsync(commentReaction.CommentId, commentReaction.UserId);

        if (existingReaction != null)
        {
            if (existingReaction.Type == commentReaction.Type)
            {
                _logger.LogInformation("Start deleting reaction of type {Type}.", existingReaction.Type);
                var resultDelete = await _commentReactionRepository.DeleteAsync(commentReaction.CommentId, commentReaction.UserId);

                return new OperationResult(resultDelete > 0, resultDelete > 0 ? "Reaction removed." : "Failed to remove reaction.");
            }
            else
            {
                _logger.LogInformation("Updating reaction from {OldType} to {NewType}", existingReaction.Type, commentReaction.Type);
                existingReaction.Type = commentReaction.Type;
                await _commentReactionRepository.UpdateAsync(existingReaction);
                return new OperationResult(true, "Reaction updated.");
            }
        }
        else
        {
            _logger.LogInformation("Adding new reaction: {Type} for CommentId: {CommentId}, UserId: {UserId}",
                commentReaction.Type, commentReaction.CommentId, commentReaction.UserId);

            var entity = _mapper.Map<CommentReactionEntity>(commentReaction);
            await _commentReactionRepository.AddAsync(entity);
            return new OperationResult(true, "Reaction added.");
        }
    }
}