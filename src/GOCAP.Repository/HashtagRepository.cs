namespace GOCAP.Repository;

[RegisterService(typeof(IHashtagRepository))]
internal class HashtagRepository(AppSqlDbContext _context, IMapper _mapper) : SqlRepositoryBase<HashTagEntity>(_context),
	IHashtagRepository
{
	private readonly IMapper _mapper = _mapper;
	private readonly AppSqlDbContext _context = _context;

	public async Task<List<Hashtag>> SearchHashtagsAsync(string prefix, int limit)
	{
		var hashtags = await _context.HashTags
			.AsNoTracking()
			.Where(h => h.Name!.StartsWith(prefix))
			.OrderBy(h => h.Name)
			.Take(limit)
			.Select(h => new Hashtag
			{
				Id = h.Id,
				Name = h.Name
			})
			.ToListAsync();
		return hashtags;
	}
}
