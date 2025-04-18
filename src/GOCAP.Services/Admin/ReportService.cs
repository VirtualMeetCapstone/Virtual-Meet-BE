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

    public async Task<byte[]> ExportUserReportAsync(DateRange range)
    {
        return await _reportRepository.ExportUserReportAsync(range);
    }

    public async Task<byte[]> ExportPostReportAsync(DateRange range)
    {
        return await _reportRepository.ExportPostReportAsync(range);
    }
}
