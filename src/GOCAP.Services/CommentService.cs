namespace GOCAP.Services;

[RegisterService(typeof(ICommentService))]
internal class CommentService(
    ICommentRepository _repository, 
    ILogger<CommentService> _logger
    ) : ServiceBase<Comment>(_repository, _logger), ICommentService
{
}
