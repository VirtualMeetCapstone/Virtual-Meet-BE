namespace GOCAP.Services.Intention;

public interface IRoomService : IServiceBase<Room>
{
    Task<RoomCount> GetRoomCountsAsync();
}