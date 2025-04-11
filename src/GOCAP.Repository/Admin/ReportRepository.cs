namespace GOCAP.Repository;

[RegisterService(typeof(IReportRepository))]
internal class ReportRepository(AppSqlDbContext _context) : IReportRepository
{
    private readonly AppSqlDbContext _context = _context;

    public async Task<UserReport> GetUserReportAsync(DateRange domain)
    {
        var result = await _context.Users
                        .GroupBy(u => 1)
                        .Select(g => new
                        {
                            TotalUsers = g.Count(),
                            NewUsers = g.Count(u => u.CreateTime >= domain.From && u.CreateTime <= domain.To),
                            BannedUsers = g.Count(u => u.IsDeleted && u.DeleteTime >= domain.From && u.DeleteTime <= domain.To)
                        })
                        .FirstOrDefaultAsync();

        if (result == null) 
        {
            return new UserReport();
        }

        double retentionRate = result.TotalUsers == 0 ? 0 : (double)result.BannedUsers / result.TotalUsers;
        double growthRate = result.TotalUsers == 0 ? 0 : (double)result.NewUsers / result.TotalUsers;

        return new UserReport
        {
            TotalUsers = result.TotalUsers,
            NewUsers = result.NewUsers,
            BannedUsers = result.BannedUsers,
            RetentionRate = retentionRate,
            GrowthRate = growthRate
        };
    }

    public async Task<PostReport> GetPostReportAsync(DateRange range)
    {
        var result = await _context.Posts
            .GroupBy(p => 1)
            .Select(g => new
            {
                TotalPosts = g.Count(),
                NewPosts = g.Count(p => p.CreateTime >= range.From && p.CreateTime <= range.To),
            })
            .FirstOrDefaultAsync();

        if (result == null)
        {
            return new PostReport(); 
        }

        double growthRate = result.TotalPosts == 0 ? 0 : (double)result.NewPosts / result.TotalPosts;

        return new PostReport
        {
            TotalPosts = result.TotalPosts,
            NewPosts = result.NewPosts,
            DeletedPosts = 0,
            GrowthRate = growthRate
        };
    }

}
