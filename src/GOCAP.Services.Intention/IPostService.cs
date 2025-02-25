namespace GOCAP.Services.Intention;

public interface IPostService : IServiceBase<Post>
{
    Task<QueryResult<Post>> GetWithPagingAsync(QueryInfo queryInfo);
    Task<Post> GetDetailByIdAsync(Guid id);
    Task<QueryResult<Post>> GetPostsUserReactedAsync(Guid userId, QueryInfo queryInfo);
    Task<QueryResult<Post>> GetPostByUserIdAsync(Guid userId, QueryInfo queryInfo);
}
