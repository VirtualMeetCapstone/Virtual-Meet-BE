namespace GOCAP.Services;

[RegisterService(typeof(ICommentService))]
internal class CommentService(
    ICommentRepository _repository,
    IPostRepository _postRepository,
    IUserRepository _userRepository,
    ILogger<CommentService> _logger
    ) : ServiceBase<Comment>(_repository, _logger), ICommentService
{
    public override async Task<Comment> AddAsync(Comment domain)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Comment).Name);
        domain.InitCreation();

        // Check whether post id and author existed or not.
        var isPostExists = await _postRepository.CheckExistAsync(domain.PostId);
        var author = await _userRepository.GetByIdAsync(domain.Author.Id);

        if (!isPostExists)
        {
            throw new ResourceNotFoundException($"Post {domain.PostId} does not found.");
        }

        domain.Author = new CommentAuthor
        {
            Id = author.Id,
            Name = author.Name,
            Picture = author.Picture
        };

        return await _repository.AddAsync(domain);
    }
    public async Task<QueryResult<Comment>> GetByPostIdWithPagingAsync(Guid postId, QueryInfo queryInfo)
    {
        return await _repository.GetByPostIdWithPagingAsync(postId, queryInfo);
    }
}
