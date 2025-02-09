namespace GOCAP.Services.Intention;

public interface ICommentService : IServiceBase<Comment>
{
    Task<QueryResult<Comment>> GetByPostIdWithPagingAsync(Guid postId, QueryInfo queryInfo);
}
