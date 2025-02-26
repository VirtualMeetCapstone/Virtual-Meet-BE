namespace GOCAP.Repository.Intention;

public interface IHashTagRepository : ISqlRepositoryBase<HashTagEntity>
{
	Task<List<HashTag>>  SearchHashTagsAsync(string prefix, int limit);
}
