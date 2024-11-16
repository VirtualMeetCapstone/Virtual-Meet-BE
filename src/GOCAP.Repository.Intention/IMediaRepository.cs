namespace GOCAP.Repository.Intention;

public interface IMediaRepository : IRepositoryBase<Media>
{
    Task<IEnumerable<Media>> GetByPostIdAsync(Post post);
}
