namespace GOCAP.Repository;

[RegisterService(typeof(IRoomHashtagRepository))]
internal class RoomHashtagRepository(AppSqlDbContext _context, IMapper _mapper) : SqlRepositoryBase<RoomHashTagEntity>(_context),
	IRoomHashtagRepository
{
	private readonly IMapper _mapper = _mapper;
	private readonly AppSqlDbContext _context = _context;

	public async Task<QueryResult<Room>> GetRoomHashtagsWithPagingAsync(string tag, QueryInfo queryInfo)
	{
		var totalItems = await _context.RoomHashTags
			.Where(r => r.HashTag!.Name!.StartsWith(tag))
			.CountAsync();

		var roomHashtags = await (from rh in _context.RoomHashTags
								  join h in _context.HashTags on rh.HashTagId equals h.Id
								  join r in _context.Rooms on rh.RoomId equals r.Id
								  join u in _context.Users on r.OwnerId equals u.Id into ownerGroup
								  from owner in ownerGroup.DefaultIfEmpty() 
								  where h.Name!.StartsWith(tag)
								  select new Room
								  {
									  Id = r.Id,
									  OwnerId = r.OwnerId,
									  Owner = owner != null ? new User
									  {
										  Name = owner.Name,
										  Picture = JsonHelper.Deserialize<Media>(owner.Picture)
									  } : null,
									  Topic = r.Topic,
									  Description = r.Description,
									  MaximumMembers = r.MaximumMembers,
									  Medias = JsonHelper.Deserialize<List<Media>>(r.Medias),
									  Status = r.Status,
									  CreateTime = r.CreateTime,
									  Members = (from rm in _context.RoomMembers
												 join mu in _context.Users on rm.UserId equals mu.Id
												 where rm.RoomId == r.Id
												 select new User
												 {
													 Name = mu.Name,
													 Picture = JsonHelper.Deserialize<Media>(mu.Picture)
												 }).ToList()
								  })
						  .Skip(queryInfo.Skip)
						  .Take(queryInfo.Top)
						  .ToListAsync();


		return new QueryResult<Room>
		{
			Data = roomHashtags,
			TotalCount = totalItems
		};
	}
}

