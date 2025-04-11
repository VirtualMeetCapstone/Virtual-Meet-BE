
namespace GOCAP.Repository.Intention;

public interface IReportRepository
{
    Task<UserReport> GetUserReportAsync(DateRange domain);
    Task<PostReport> GetPostReportAsync(DateRange domain);
}
