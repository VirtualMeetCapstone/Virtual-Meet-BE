namespace GOCAP.Repository;

[RegisterService(typeof(IRoomFavouriteRepository))]
internal class RoomFavouriteRepository(AppSqlDbContext context,
     IMapper mapper) : SqlRepositoryBase<RoomFavourite, RoomFavouriteEntity>(context, mapper), IRoomFavouriteRepository
{
    private readonly AppSqlDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    public override async Task<RoomFavourite> AddAsync(RoomFavourite roomFavourite)
    {
        roomFavourite.CreateTime = DateTime.Now.Ticks;
        var roomFavouriteEntity = _mapper.Map<RoomFavouriteEntity>(roomFavourite);
        await _context.RoomFavourites.AddAsync(roomFavouriteEntity);
        await _context.SaveChangesAsync();
        return roomFavourite;
    }

}
