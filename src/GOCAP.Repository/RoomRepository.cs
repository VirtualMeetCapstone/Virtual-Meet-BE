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

        if (queryInfo.OrderBy is not null)
        {
            if (queryInfo.OrderType == OrderType.Ascending)
            {
                roomQuery = roomQuery.OrderBy(x => EF.Property<object>(x, queryInfo.OrderBy));
            }
            else if (queryInfo.OrderType == OrderType.Descending)
            {
                roomQuery = roomQuery.OrderByDescending(x => EF.Property<object>(x, queryInfo.OrderBy));
            }
        }

        var rooms = await roomQuery.Include(r => r.Owner)
                                   .Include(r => r.Members)
                                   .ThenInclude(rm => rm.User)
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
            Medias = r.Medias,
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
}
