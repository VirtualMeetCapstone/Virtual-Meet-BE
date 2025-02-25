namespace GOCAP.Services.Intention;

public interface IHashtagService : IServiceBase<Hashtag>
{
	Task<List<Hashtag>> SearchHashtagsAsync(string prefix, int limit);
}
