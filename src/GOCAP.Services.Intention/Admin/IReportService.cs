namespace GOCAP.Services.Intention;

public interface IReportService
{
    Task<PostReport> GetPostReportAsync(DateRange domain);
    Task<UserReport> GetUserReportAsync(DateRange domain);
    Task<byte[]> ExportUserReportAsync(DateRange range);
    Task<byte[]> ExportPostReportAsync(DateRange range);
    Task<UserReport> GetUserReportNewAsync(DateRange domain);

}
