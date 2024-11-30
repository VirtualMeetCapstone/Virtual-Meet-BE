namespace GOCAP.Services;

internal class CommentService(
    ICommentRepository _repository, 
    ILogger<CommentService> _logger
    ) : ServiceBase<Comment>(_repository, _logger), ICommentService
{
}
