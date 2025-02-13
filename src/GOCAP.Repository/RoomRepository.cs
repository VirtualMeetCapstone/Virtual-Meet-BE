using System.Text.Json;

namespace GOCAP.Repository;

[RegisterService(typeof(IRoomRepository))]
internal class RoomRepository(
    AppSqlDbContext context,
     IMapper mapper) : SqlRepositoryBase<Room, RoomEntity>(context, mapper), IRoomRepository
{
    private readonly AppSqlDbContext _context = context;

    public override async Task<QueryResult<Room>> GetByPageAsync(QueryInfo queryInfo)
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
            Owner = new User
            {
                Name = r.Owner?.Name ?? string.Empty,
                Bio = r.Owner?.Picture,
            },
            Topic = r.Topic,
            Description = r.Description,
            MaximumMembers = r.MaximumMembers,
            Medias = JsonSerializer.Deserialize<List<Media>>(r.Medias ?? string.Empty) ?? [],
            Status = r.Status,
            CreateTime = r.CreateTime,
            Members = r.Members.Select(rm => new User
            {
                Name = rm.User?.Name ?? string.Empty,
                Email = rm.User?.Email,
                Picture = rm.User?.Picture
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
