namespace GOCAP.Services;

[RegisterService(typeof(ICommentService))]
internal class CommentService(
    ICommentRepository _repository,
    IPostRepository _postRepository,
    IUserRepository _userRepository,
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
    public async Task<QueryResult<Comment>> GetByPostIdWithPagingAsync(Guid postId, QueryInfo queryInfo)
    {
        var result = await _repository.GetByPostIdWithPagingAsync(postId, queryInfo);
        return _mapper.Map<QueryResult<Comment>>(result);
    }

   public async Task<QueryResult<Comment>> GetRepliesAsyncWithPagingAsync(Guid commentId, QueryInfo queryInfo)
    {
        var result = await _repository.GetRepliesAsyncWithPagingAsync(commentId, queryInfo);
        return _mapper.Map<QueryResult<Comment>>(result);
    }
}
