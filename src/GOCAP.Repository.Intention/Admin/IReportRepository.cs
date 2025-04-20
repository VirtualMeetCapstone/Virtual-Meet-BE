
namespace GOCAP.Repository.Intention;

public interface IReportRepository
{
    Task<UserReport> GetUserReportAsync(DateRange domain);
    Task<PostReport> GetPostReportAsync(DateRange domain);
    Task<List<User>> GetUsersInRangeAsync(DateRange range);
    Task<List<Post>> GetPostsInRangeAsync(DateRange range);
    Task<byte[]> ExportUserReportAsync(DateRange range);
    Task<byte[]> ExportPostReportAsync(DateRange range);
    Task<UserReport> GetUserReportNewAsync(DateRange domain);
}
