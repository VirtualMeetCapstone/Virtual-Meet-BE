namespace GOCAP.Admin.Api.Controllers;

[Route("admin/dashboard")]
public class DashBoardController(IUserService _userService,
    IRoomService _roomService,
    IGroupService _groupService,
    IMapper _mapper) : ApiControllerBase
{
    [HttpGet("count-statistics")]
    public async Task<CountStatisticsModel> StatisticsCountAsync()
    {

        var userCounts = await _userService.GetUserCountsAsync();
        var roomCounts = await _roomService.GetRoomCountsAsync();
        var groupCounts = await _groupService.GetGroupCountsAsync();

        var countStatistics = new CountStatistics
        {
            // User
            UserTotal = userCounts.Total,
            UserActive = userCounts.Active,
            UserInActive = userCounts.InActive,
            UserDeleted = userCounts.Deleted,

            //Room
            RoomTotal = roomCounts.Total,
            RoomAvailable = roomCounts.Available,
            RoomOccupied = roomCounts.Occupied,
            RoomReserved = roomCounts.Reserved,
            RoomOutOfService = roomCounts.OutOfService,

            //Group
            GroupTotal = groupCounts.Total

            //
            //.............
            //
        };
        var result = _mapper.Map<CountStatisticsModel>(countStatistics);

        return result;
    }
}


