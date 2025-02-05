namespace GOCAP.Repository;

[RegisterService(typeof(IRoomFavouriteRepository))]
internal class RoomFavouriteRepository(AppSqlDbContext context,
     IMapper mapper) : SqlRepositoryBase<RoomFavourite, RoomFavouriteEntity>(context, mapper), IRoomFavouriteRepository
{
    private readonly AppSqlDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    public async Task<RoomFavourite?> GetByRoomAndUserAsync(Guid roomId, Guid userId)
    {
        var entity = await _context.RoomFavourites.FirstOrDefaultAsync
                                                (f => f.RoomId == roomId
                                                 && f.UserId == userId);
        return _mapper.Map<RoomFavourite?>(entity);
    }
}
