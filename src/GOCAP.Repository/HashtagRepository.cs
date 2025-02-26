namespace GOCAP.Repository;

[RegisterService(typeof(IHashTagRepository))]
internal class HashTagRepository(AppSqlDbContext _context, IMapper _mapper) : SqlRepositoryBase<HashTagEntity>(_context),
	IHashTagRepository
{
	private readonly IMapper _mapper = _mapper;
	private readonly AppSqlDbContext _context = _context;

	public async Task<List<HashTag>> SearchHashTagsAsync(string prefix, int limit)
	{
		var hashtags = await _context.HashTags
			.AsNoTracking()
			.Where(h => h.Name!.StartsWith(prefix))
			.OrderBy(h => h.Name)
			.Take(limit)
			.Select(h => new HashTag
			{
				Id = h.Id,
				Name = h.Name
			})
			.ToListAsync();
		return hashtags;
	}
}
