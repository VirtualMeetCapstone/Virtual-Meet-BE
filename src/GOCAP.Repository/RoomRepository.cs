namespace GOCAP.Repository;

[RegisterService(typeof(IRoomRepository))]
internal class RoomRepository(
    AppSqlDbContext context,
     IBlobStorageService _blobStorageService
     ) : SqlRepositoryBase<RoomEntity>(context), IRoomRepository
{
    private readonly AppSqlDbContext _context = context;

    public async Task<QueryResult<Room>> GetWithPagingAsync(QueryInfo queryInfo)
    {
        var roomQuery = _context.Rooms.AsQueryable();

        var rooms = await roomQuery.Include(r => r.Owner)
                                   .Include(r => r.Members)
                                   .ThenInclude(rm => rm.User)
                                   .OrderByDescending(r => r.CreateTime)
                                   .Skip(queryInfo.Skip)
                                   .Take(queryInfo.Top)
                                   .ToListAsync();

        var roomsDetail = rooms.Select(r => new Room
        {
            Id = r.Id,
            OwnerId = r.OwnerId,
            Owner = new User
            {
                Name = r.Owner?.Name ?? string.Empty,
                Picture = JsonHelper.Deserialize<Media>(r.Owner?.Picture),
            },
            Topic = r.Topic,
            Description = r.Description,
            MaximumMembers = r.MaximumMembers,
            Medias = JsonHelper.Deserialize<List<Media>>(r.Medias),
            Status = r.Status,
            CreateTime = r.CreateTime,
            Members = r.Members.Select(rm => new User
            {
                Name = rm.User?.Name ?? string.Empty,
                Email = rm.User?.Email,
                Picture = JsonHelper.Deserialize<Media>(rm.User?.Picture)
            })
        });

        int totalItems = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalItems = await _context.Rooms.CountAsync();
        }

        return new QueryResult<Room>
        {
            Data = roomsDetail,
            TotalCount = totalItems
        };

    }

    public override async Task<bool> UpdateAsync(RoomEntity roomEntity)
    {
        var entity = await GetEntityByIdAsync(roomEntity.Id);
        if (!string.IsNullOrEmpty(entity.Medias))
        {
            var medias = JsonHelper.Deserialize<List<Media>>(entity.Medias);
            await _blobStorageService.DeleteFilesByUrlsAsync(medias?.Select(m => m.Url).ToList());
        }
        entity.Topic = roomEntity.Topic;
        entity.Description = roomEntity.Description;
        entity.MaximumMembers = roomEntity.MaximumMembers;
        entity.Medias = roomEntity.Medias;
        entity.LastModifyTime = roomEntity.LastModifyTime;
        _context.Entry(entity).State = EntityState.Modified;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<RoomCount> GetRoomCountsAsync()
    {
        var counts = await _context.Rooms
            .GroupBy(r => 1)
            .Select(g => new RoomCount
            {
                Total = g.Count(),
                Available = g.Count(r => r.Status == RoomStatusType.Available),
                Occupied = g.Count(r => r.Status == RoomStatusType.Occupied),
                Reserved = g.Count(r => r.Status == RoomStatusType.Reserved),
                OutOfService = g.Count(r => r.Status == RoomStatusType.OutOfService)
            })
            .FirstOrDefaultAsync();

        return counts ?? new RoomCount();
    }
}
