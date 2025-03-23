namespace GOCAP.Repository;

[RegisterService(typeof(IRoomRepository))]
internal class RoomRepository(
    AppSqlDbContext context
     ) : SqlRepositoryBase<RoomEntity>(context), IRoomRepository
{
    private readonly AppSqlDbContext _context = context;

    public async Task<QueryResult<Room>> GetWithPagingAsync(QueryInfo queryInfo)
    {
        var query = _context.Rooms.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            query = query.Where(r => EF.Functions.Collate(r.Topic, "Latin1_General_CI_AI").Contains(queryInfo.SearchText.Trim()));
        }

        var totalItems = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        var rooms = await query
            .OrderByDescending(r => r.CreateTime)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(r => new Room
            {
                Id = r.Id,
                OwnerId = r.OwnerId,
                Owner = r.Owner != null ? new User
                {
                    Name = r.Owner.Name,
                    Picture = JsonHelper.Deserialize<Media>(r.Owner.Picture),
                } : null,
                Topic = r.Topic,
                Description = r.Description,
                MaximumMembers = r.MaximumMembers,
                Medias = JsonHelper.Deserialize<List<Media>>(r.Medias),
                Status = r.Status,
                CreateTime = r.CreateTime,
                Members = _context.RoomMembers
                    .Where(rm => rm.RoomId == r.Id)
                    .Join(_context.Users, rm => rm.UserId, u => u.Id, (rm, u) => new User
                    {
                        Name = u.Name,
                        Picture = JsonHelper.Deserialize<Media>(u.Picture)
                    })
                    .ToList()
            })
            .ToListAsync();

        return new QueryResult<Room>
        {
            Data = rooms,
            TotalCount = totalItems
        };
    }

    public async Task<RoomCount> GetRoomCountsAsync()
    {
        var counts = await _context.Rooms
                    .AsNoTracking()
                    .Select(r => new
                    {
                        r.Status
                    })
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

    public async Task<Room> GetDetailByIdAsync(Guid id)
    {
        var room = await _context.Rooms
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new Room
            {
                Id = r.Id,
                OwnerId = r.OwnerId,
                Owner = _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == r.OwnerId)
                    .Select(u => new User
                    {
                        Name = u.Name,
                        Picture = JsonHelper.Deserialize<Media>(u.Picture),
                    })
                    .FirstOrDefault(),
                Topic = r.Topic,
                Description = r.Description,
                MaximumMembers = r.MaximumMembers,
                Medias = JsonHelper.Deserialize<List<Media>>(r.Medias),
                Status = r.Status,
                CreateTime = r.CreateTime,
                Members = _context.RoomMembers
                    .Where(rm => rm.RoomId == r.Id)
                    .Join(_context.Users, rm => rm.UserId, u => u.Id, (rm, u) => new User
                    {
                        Name = u.Name,
                        Picture = JsonHelper.Deserialize<Media>(u.Picture)
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        return room ?? throw new ResourceNotFoundException($"Room {id} was not found.");
    }
}
