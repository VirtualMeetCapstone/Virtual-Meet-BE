namespace GOCAP.Services.Intention;

public interface IReportService
{
    Task<PostReport> GetPostReportAsync(DateRange domain);
    Task<UserReport> GetUserReportAsync(DateRange domain);
}
