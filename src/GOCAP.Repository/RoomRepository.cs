namespace GOCAP.Repository;

[RegisterService(typeof(IRoomRepository))]
internal class RoomRepository(
    AppSqlDbContext context
     ) : SqlRepositoryBase<RoomEntity>(context), IRoomRepository
{
    private readonly AppSqlDbContext _context = context;

    public async Task<QueryResult<Room>> GetWithPagingAsync(QueryInfo queryInfo)
    {
        var query = _context.Rooms
            .AsNoTracking()
            .AsQueryable();

        // Filtering
        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            var keyword = queryInfo.SearchText.Trim();
            query = query.Where(r => EF.Functions.Collate(r.Topic, "Latin1_General_CI_AI").Contains(keyword));
        }

        // Total count
        var totalItems = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        // Fetch data in one query, no C# method calls inside
        var roomData = await query
            .OrderByDescending(r => r.CreateTime)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Join(_context.Users, r => r.OwnerId, u => u.Id, (r, u) => new
            {
                Room = r,
                Owner = new { u.Name, u.Picture }
            })
            .GroupJoin(
                _context.RoomMembers.Join(_context.Users, rm => rm.UserId, u => u.Id, (rm, u) => new
                {
                    rm.RoomId,
                    u.Name,
                    u.Picture
                }),
                ru => ru.Room.Id,
                member => member.RoomId,
                (ru, members) => new
                {
                    ru.Room,
                    ru.Owner,
                    Members = members.ToList()
                })
            .ToListAsync();
        Console.WriteLine(query.ToQueryString());
        // Now deserialize outside EF (C# land)
        var resultRooms = roomData.Select(r => new Room
        {
            Id = r.Room.Id,
            OwnerId = r.Room.OwnerId,
            Owner = r.Owner != null ? new User
            {
                Name = r.Owner.Name,
                Picture = JsonHelper.Deserialize<Media>(r.Owner.Picture)
            } : null,
            Topic = r.Room.Topic,
            Description = r.Room.Description,
            MaximumMembers = r.Room.MaximumMembers,
            Medias = JsonHelper.Deserialize<List<Media>>(r.Room.Medias),
            Status = r.Room.Status,
            CreateTime = r.Room.CreateTime,
            Members = [.. r.Members.Select(m => new User
            {
                Name = m.Name,
                Picture = JsonHelper.Deserialize<Media>(m.Picture)
            })]
        }).ToList();

        return new QueryResult<Room>
        {
            Data = resultRooms,
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

    public async Task<Room?> GetDetailByIdAsync(Guid id)
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

        return room;
    }
}
