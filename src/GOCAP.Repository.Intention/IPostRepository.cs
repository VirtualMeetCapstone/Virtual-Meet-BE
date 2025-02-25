namespace GOCAP.Repository.Intention;

public interface IPostRepository : ISqlRepositoryBase<PostEntity>
{
    Task<QueryResult<Post>> GetWithPagingAsync(QueryInfo queryInfo);
    Task<Post> GetDetailByIdAsync(Guid id);
    Task<QueryResult<Post>> GetPostByUserIdAsync(Guid userId, QueryInfo queryInfo);
    Task<QueryResult<Post>> GetPostsUserReactedAsync(Guid userId, QueryInfo queryInfo);
}
