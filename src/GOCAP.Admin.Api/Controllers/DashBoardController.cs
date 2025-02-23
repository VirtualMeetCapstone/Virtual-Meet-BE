namespace GOCAP.Admin.Api.Controllers;

[Route("admin/dashboard")]
public class DashBoardController(ICountStatisticService _countStatisticService,
    IMapper _mapper) : ApiControllerBase
{
    [HttpGet("count-statistics")]
    public async Task<CountStatisticsModel> StatisticsCountAsync()
    {
        var countStatistics = await _countStatisticService.GetStatisticsCountAsync();
        var result = _mapper.Map<CountStatisticsModel>(countStatistics);
        return result;
    }
}
