using GOCAP.ExternalServices;
using System.Text.Json;

namespace GOCAP.Repository;

[RegisterService(typeof(IRoomRepository))]
internal class RoomRepository(
    AppSqlDbContext context,
     IMapper mapper,
     IBlobStorageService _blobStorageService
     ) : SqlRepositoryBase<Room, RoomEntity>(context, mapper), IRoomRepository
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
            OwnerId = r.OwnerId,
            Owner = new User
            {
                Name = r.Owner?.Name ?? string.Empty,
                Bio = r.Owner?.Picture,
            },
            Topic = r.Topic,
            Description = r.Description,
            MaximumMembers = r.MaximumMembers,
            Medias = !string.IsNullOrEmpty(r.Medias)
                        ? JsonSerializer.Deserialize<List<Media>>(r.Medias)
                        : null,
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

    public override async Task<bool> UpdateAsync(Guid id, Room domain)
    {
        var entity = await GetEntityByIdAsync(id);
        if (entity.Medias is not null)
        {
            var medias = JsonSerializer.Deserialize<List<Media>>(entity.Medias);
            await _blobStorageService.DeleteFilesByUrlsAsync(medias?.Select(m => m.Url).ToList());
        }
        entity.Topic = domain.Topic;
        entity.Description = domain.Description;
        entity.MaximumMembers = domain.MaximumMembers;
        if (domain.Medias is not null || domain.Medias?.Count > 0)
        {
            entity.Medias = JsonSerializer.Serialize(domain.Medias);
        }
        entity.LastModifyTime = domain.LastModifyTime;
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
