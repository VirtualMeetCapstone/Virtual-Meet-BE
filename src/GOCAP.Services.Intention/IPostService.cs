namespace GOCAP.Services.Intention;

public interface IPostService : IServiceBase<Post>
{
    Task<Post> UploadPost(Post post);
}
