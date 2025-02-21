
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
        var totalItems = queryInfo.NeedTotalCount
                ? await _context.Rooms.CountAsync()
                : 0;

        var rooms = await _context.Rooms
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
            .Select(r => new
            {
                r.Status
            })
            .AsNoTracking()
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
    public async Task<Room> GetDetailIdAsync(Guid id)
    => await (from r in _context.Rooms.AsNoTracking()
              join m in _context.RoomMembers.AsNoTracking() on r.Id equals m.RoomId
              join u in _context.Users.AsNoTracking() on m.UserId equals u.Id
              where r.Id == id
              select new Room
              {
                  Id = r.Id,
                  OwnerId = r.OwnerId,
                  Owner = new User
                  {
                      Name = u.Name,
                      Picture = JsonHelper.Deserialize<Media>(u.Picture),
                  },
                  Topic = r.Topic,
                  Description = r.Description,
                  MaximumMembers = r.MaximumMembers,
                  Medias = JsonHelper.Deserialize<List<Media>>(r.Medias),
                  Status = r.Status,
                  CreateTime = r.CreateTime,
                  Members = _context.RoomMembers
                      .AsNoTracking() 
                      .Where(rm => rm.RoomId == r.Id)
                      .Join(_context.Users.AsNoTracking(), rm => rm.UserId, u => u.Id, (rm, u) => new User
                      {
                          Name = u.Name,
                          Picture = JsonHelper.Deserialize<Media>(u.Picture)
                      }).ToList()
              }).FirstOrDefaultAsync()
        ?? throw new ResourceNotFoundException($"Room {id} was not found.");
}
