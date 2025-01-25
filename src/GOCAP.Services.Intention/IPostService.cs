namespace GOCAP.Services.Intention;

public interface IPostService : IServiceBase<UserPost>
{
    Task<UserPost> UploadPost(UserPost post);
}
