using GOCAP.Database;
using GOCAP.Database.Common.Entities;
using GOCAP;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace GOCAP.Api.Controllers
{
    [ApiController]
    [Route("/report")]
    public class ReportController : ControllerBase
    {
        private readonly AppMongoDbContext _dbContext;

        public ReportController(AppMongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("report-for-ban")]
        public async Task<IActionResult> ReportUser([FromBody] ReportUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingReport = await _dbContext.Reports.Find(r =>
                r.TargetId == request.TargetId &&
                r.ReporterId == request.ReporterId
            ).FirstOrDefaultAsync();

            if (existingReport != null)
            {
                return Conflict(new
                {
                    message = "You have already reported this user."
                });
            }

            var report = new ReportEntity
            {
                Id = Guid.NewGuid(),
                TargetId = request.TargetId,
                ReporterId = request.ReporterId,
                ReportType = request.ReportType,
                Description = request.Description,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Reports.InsertOneAsync(report);

            return Ok(new
            {
                message = "Report submitted successfully.",
                reportId = report.Id
            });
        }
        [HttpGet("get-all-reports")]
        public async Task<IActionResult> GetAllReports()
        {
            // Lấy tất cả báo cáo từ MongoDB
            var reports = await _dbContext.Reports.Find(_ => true).ToListAsync();

            if (reports == null || reports.Count == 0)
            {
                return NotFound(new { message = "No reports found" });
            }

            return Ok(reports);
        }
    }
}
