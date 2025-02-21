namespace GOCAP.Repository;

[RegisterService(typeof(IRoomFavouriteRepository))]
internal class RoomFavouriteRepository(AppSqlDbContext context) : SqlRepositoryBase<RoomFavouriteEntity>(context), IRoomFavouriteRepository
{
	private readonly AppSqlDbContext _context = context;
	public async Task<RoomFavouriteEntity?> GetByRoomAndUserAsync(Guid roomId, Guid userId)
	{
		var entity = await _context.RoomFavourites.FirstOrDefaultAsync
												(f => f.RoomId == roomId
												 && f.UserId == userId);
		return entity;
	}

	public async Task<QueryResult<Room>> GetRoomFavouriteAsync(Guid userId, QueryInfo queryInfo)
	{
		var rooms = await _context.RoomFavourites
								  .Where(r => r.UserId == userId)
								  .OrderByDescending(r => r.CreateTime)
								  .Skip(queryInfo.Skip)
								  .Take(queryInfo.Top)
								  .Select(r => new Room
								  {
									  Id = r.Room!.Id,
									  OwnerId = r.Room.OwnerId,
									  Owner = new User
									  {
										  Name = r.Room.Owner!.Name ?? string.Empty,
										  Picture = JsonHelper.Deserialize<Media>(r.Room.Owner.Picture),
									  },
									  Topic = r.Room.Topic,
									  Medias = JsonHelper.Deserialize<List<Media>>(r.Room.Medias),
								  })
								  .ToListAsync();

		var totalItems = queryInfo.NeedTotalCount ? rooms.Count : 0;

		return new QueryResult<Room>
		{
			Data = rooms,
			TotalCount = totalItems
		};
	}
}
