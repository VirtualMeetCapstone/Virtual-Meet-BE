namespace GOCAP.Repository.Intention;

public interface IRoomRepository : ISqlRepositoryBase<Room>
{
    Task<RoomCount> GetRoomCountsAsync();
}
