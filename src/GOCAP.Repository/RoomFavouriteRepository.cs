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
}
