namespace GOCAP.Services.Intention;

public interface IPostService : IServiceBase<Post>
{
    Task<Post> GetDetailByIdAsync(Guid id);
}
