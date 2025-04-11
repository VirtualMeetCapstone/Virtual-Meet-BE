namespace GOCAP.Services;

[RegisterService(typeof(IReportService))]
internal class ReportService (IReportRepository _reportRepository) : IReportService
{
    public async Task<UserReport> GetUserReportAsync(DateRange domain)
    {
        return await _reportRepository.GetUserReportAsync(domain);
    }
    public async Task<PostReport> GetPostReportAsync(DateRange domain)
    {
        return await _reportRepository.GetPostReportAsync(domain);
    }
}
