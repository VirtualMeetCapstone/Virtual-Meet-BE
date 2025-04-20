namespace GOCAP.Api.Controllers;

[Route("report")]
public class ReportsController(IMapper _mapper, IReportService _service) : ApiControllerBase
{
    [HttpGet("user")]
    public async Task<UserReportModel> GetUserReport([FromQuery] DateRangeModel model)
    {
        var domain = _mapper.Map<DateRange>(model);
        var result = await _service.GetUserReportNewAsync(domain);
        return _mapper.Map<UserReportModel>(result);
    }

    [HttpGet("post")]
    public async Task<PostReportModel> GetPostReport([FromQuery] DateRangeModel model)
    {
        var domain = _mapper.Map<DateRange>(model);
        var result = await _service.GetPostReportAsync(domain);
        return _mapper.Map<PostReportModel>(result);
    }

    [HttpGet("user/excel")]
    public async Task<IActionResult> ExportUserReportExcel([FromQuery] DateRangeModel model)
    {
        var domain = _mapper.Map<DateRange>(model);
        var file = await _service.ExportUserReportAsync(domain);
        return File(file, FormatExcel.ExcelMimeType, FormatExcel.UserReportName);
    }

    [HttpGet("post/excel")]
    public async Task<IActionResult> ExportPostReportExcel([FromQuery] DateRangeModel model)
    {
        var domain = _mapper.Map<DateRange>(model);
        var file = await _service.ExportPostReportAsync(domain);
        return File(file, FormatExcel.ExcelMimeType, FormatExcel.PostReportName);
    }
}
