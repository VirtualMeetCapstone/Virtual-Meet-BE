namespace GOCAP.Api.Controllers;

[Route("report")]
public class ReportsController (IMapper _mapper, IReportService _service) : ApiControllerBase
{
    [HttpGet("user")]
    public async Task<UserReportModel> GetUserReport([FromQuery] DateRangeModel model)
    {
        var domain = _mapper.Map<DateRange>(model);
        var result = await _service.GetUserReportAsync(domain);
        return _mapper.Map<UserReportModel>(result);
    }

    [HttpGet("post")]
    public async Task<PostReportModel> GetPostReport([FromQuery] DateRangeModel model)
    {
        var domain = _mapper.Map<DateRange>(model);
        var result = await _service.GetPostReportAsync(domain);
        return _mapper.Map<PostReportModel>(result);
    }
}
