namespace GOCAP.Services;

[RegisterService(typeof(ICountStatisticService))]
internal class CountStatisticService(
    IUserRepository _userRepository,
    IRoomRepository _roomRepository,
    IGroupRepository _groupRepository
    ) : ICountStatisticService
{
    public async Task<CountStatistics> GetStatisticsCountAsync()
    {
        var userCounts = await _userRepository.GetUserCountsAsync();
        var roomCounts = await _roomRepository.GetRoomCountsAsync();
        var groupCounts = await _groupRepository.GetGroupCountsAsync();

        var result = new CountStatistics
        {
            // User
            UserTotal = userCounts.Total,
            UserActive = userCounts.Active,
            UserInActive = userCounts.InActive,
            UserDeleted = userCounts.Deleted,

            // Room
            RoomTotal = roomCounts.Total,
            RoomAvailable = roomCounts.Available,
            RoomOccupied = roomCounts.Occupied,
            RoomReserved = roomCounts.Reserved,
            RoomOutOfService = roomCounts.OutOfService,

            // Group
            GroupTotal = groupCounts.Total
        };

        return result;
    }
}
