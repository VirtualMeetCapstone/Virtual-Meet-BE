namespace GOCAP.Repository;

[RegisterService(typeof(IRoomFavouriteRepository))]
internal class RoomFavouriteRepository(AppSqlDbContext context,
     IMapper mapper) : SqlRepositoryBase<RoomFavourite, RoomFavouriteEntity>(context, mapper), IRoomFavouriteRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<bool> CheckExistAsync(Guid roomId, Guid userId)
    {
        return await _context.RoomFavourites.AnyAsync(rf => rf.RoomId == roomId
                                                        && rf.UserId == userId);
    }

    public async Task<int> DeleteAsync(Guid roomId, Guid userId)
    {
        var roomFavourite = await _context.RoomFavourites
            .FirstOrDefaultAsync(rf => rf.RoomId == roomId && rf.UserId == userId)
            ?? throw new ResourceNotFoundException("This room favourite does not exists.");
        _context.RoomFavourites.Remove(roomFavourite);
        return await _context.SaveChangesAsync();
    }
}
