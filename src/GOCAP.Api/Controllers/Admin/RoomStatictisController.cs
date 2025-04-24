namespace GOCAP.Api.Controllers;

[Route("RoomStatictis")]
public class RoomStatictisController(IRoomStatisticService _roomStatistic) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetWithPaging([FromQuery] QueryInfo queryInfo)
    {
        var result = await _roomStatistic.GetWithPagingAsync(queryInfo);
        return Ok(result);
    }

    // GET: /RoomStatistics/{roomId}
    [HttpGet("{roomId}")]
    public async Task<IActionResult> GetByRoomId(string roomId)
    {
        var entity = await _roomStatistic.GetByRoomIdAsync(roomId);
        if (entity == null)
            return NotFound();

        return Ok(entity);
    }

    // GET: /RoomStatistics/date-range?startDate=2024-01-01&endDate=2024-01-31
    [HttpGet("date-range")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var list = await _roomStatistic.GetByDateRangeAsync(startDate, endDate);
        return Ok(list);
    }
}
