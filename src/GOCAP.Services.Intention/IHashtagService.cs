namespace GOCAP.Services.Intention;

public interface IHashTagService : IServiceBase<HashTag>
{
	Task<List<HashTag>> SearchHashTagsAsync(string prefix, int limit);
}
