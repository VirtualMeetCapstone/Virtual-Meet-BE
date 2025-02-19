namespace GOCAP.Repository.Intention;

public interface IPostRepository : ISqlRepositoryBase<PostEntity>
{
    Task<Post> GetDetailByIdAsync(Guid id);
}
