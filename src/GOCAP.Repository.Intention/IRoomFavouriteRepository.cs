namespace GOCAP.Repository.Intention;

public interface IRoomFavouriteRepository : ISqlRepositoryBase<RoomFavourite>
{
    Task<bool> CheckExistAsync(Guid roomId, Guid userId);
    Task<int> DeleteAsync(Guid roomId, Guid userId);
}
