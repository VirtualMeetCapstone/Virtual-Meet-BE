namespace GOCAP.Repository;

[RegisterService(typeof(IReportRepository))]
internal class ReportRepository(AppSqlDbContext _context, IMapper _mapper
    , IExcelExportService _excelExportService
    ) : IReportRepository
{
    private readonly AppSqlDbContext _context = _context;
    private readonly IMapper _mapper = _mapper;
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

        double retentionRate = result.TotalUsers == 0 ? 0 : 1 - ((double)result.BannedUsers / result.NewUsers);
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

    public async Task<List<User>> GetUsersInRangeAsync(DateRange range)
    {
        var entities = await _context.Users
            .Where(u => u.CreateTime >= range.From && u.CreateTime <= range.To)
            .ToListAsync();

        return _mapper.Map<List<User>>(entities);
    }

    public async Task<List<Post>> GetPostsInRangeAsync(DateRange range)
    {
        var entities = await _context.Posts
      .Where(u => u.CreateTime >= range.From && u.CreateTime <= range.To)
      .ToListAsync();

        return _mapper.Map<List<Post>>(entities);
    }
    public async Task<byte[]> ExportUserReportAsync(DateRange range)
    {
        var report = await GetUserReportAsync(range);
        var users = await GetUsersInRangeAsync(range);

        var overview = new Dictionary<string, object?>
        {
            ["Total Users"] = report.TotalUsers,
            ["New Users"] = report.NewUsers,
            ["Banned Users"] = report.BannedUsers,
            ["Retention Rate"] = report.RetentionRate,
            ["Growth Rate"] = report.GrowthRate
        };

        var columns = new List<(string, Func<User, object?>)>
    {
        ("ID", u => u.Id),
        ("Name", u => u.Name),
        ("Status", u => u.IsDeleted ? "Banned" : "Active"),
        ("Created At", u => new DateTime(u.CreateTime).ToString("yyyy-MM-dd HH:mm")),
        ("Deleted At", u => u.DeleteTime.HasValue ? new DateTime(u.DeleteTime.Value).ToString("yyyy-MM-dd HH:mm") : ""),
        ("Bio", u => u.Bio ?? "")
    };

        return _excelExportService.ExportToExcel("User List", columns, users, overview);
    }


    public async Task<byte[]> ExportPostReportAsync(DateRange range)
    {
        var report = await GetPostReportAsync(range);
        var posts = await GetPostsInRangeAsync(range);

        using var workbook = new XLWorkbook();

        // Sheet 1: Thống kê tổng quan
        var sheet1 = workbook.Worksheets.Add("Post Overview Statistics");

        sheet1.Cell(1, 1).Value = "Total Posts";
        sheet1.Cell(1, 2).Value = "New Posts";
        sheet1.Cell(1, 3).Value = "Deleted Posts";
        sheet1.Cell(1, 4).Value = "Growth Rate";

        sheet1.Cell(2, 1).Value = report.TotalPosts;
        sheet1.Cell(2, 2).Value = report.NewPosts;
        sheet1.Cell(2, 3).Value = report.DeletedPosts;
        sheet1.Cell(2, 4).Value = report.GrowthRate;

        sheet1.Range("A1:D1").Style.Font.Bold = true;
        sheet1.Columns().AdjustToContents();

        // Sheet 2: Danh sách bài viết
        var sheet2 = workbook.Worksheets.Add("Post List");

        // Header
        sheet2.Cell(1, 1).Value = "Post ID";
        sheet2.Cell(1, 2).Value = "User ID";
        sheet2.Cell(1, 3).Value = "Content";
        sheet2.Cell(1, 4).Value = "Created At";
        sheet2.Cell(1, 5).Value = "Total Reactions";
        sheet2.Cell(1, 6).Value = "Comment Count";
        sheet2.Cell(1, 7).Value = "Like Count";
        sheet2.Cell(1, 8).Value = "Dislike Count";

        for (int i = 0; i < posts.Count; i++)
        {
            var p = posts[i];
            var row = i + 2;

            sheet2.Cell(row, 1).Value = p.Id.ToString();
            sheet2.Cell(row, 2).Value = p.UserId.ToString();
            sheet2.Cell(row, 3).Value = p.Content ?? "";
            sheet2.Cell(row, 4).Value = new DateTime(p.CreateTime).ToString("yyyy-MM-dd HH:mm");
            sheet2.Cell(row, 5).Value = p.TotalReactions;
            sheet2.Cell(row, 6).Value = p.CommentCount;
            sheet2.Cell(row, 7).Value = p.ReactionCounts.ContainsKey(1) ? p.ReactionCounts[1] : 0; // Like
            sheet2.Cell(row, 8).Value = p.ReactionCounts.ContainsKey(2) ? p.ReactionCounts[2] : 0; // Dislike
        }

        sheet2.Range("A1:I1").Style.Font.Bold = true;
        sheet2.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<UserReport> GetUserReportNewAsync(DateRange domain)
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
            return new UserReport();

        double retainedUsers = result.TotalUsers - result.NewUsers;
        double previousPeriodUsers = retainedUsers + result.BannedUsers;

        double retentionRate = previousPeriodUsers == 0 ? 0 :
            (double)retainedUsers / previousPeriodUsers;

        double growthRate = result.TotalUsers == 0 ? 0 :
            (double)result.NewUsers / result.TotalUsers;

        return new UserReport
        {
            TotalUsers = result.TotalUsers,
            NewUsers = result.NewUsers,
            BannedUsers = result.BannedUsers,
            RetentionRate = retentionRate,
            GrowthRate = growthRate
        };
    }


}
