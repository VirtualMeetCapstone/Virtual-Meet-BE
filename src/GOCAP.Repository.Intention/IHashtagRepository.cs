namespace GOCAP.Repository.Intention;

public interface IHashtagRepository : ISqlRepositoryBase<HashTagEntity>
{
	Task<List<Hashtag>>  SearchHashtagsAsync(string prefix, int limit);
}
