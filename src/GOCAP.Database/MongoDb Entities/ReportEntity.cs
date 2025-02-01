namespace GOCAP.Database;

[BsonCollection("Reports")]
public class ReportEntity : EntityMongoBase
{
    public Guid TargetId { get; set; }
    public string? ReportType { get; set; }
    public string? Description { get; set; }
    public long CreateTime { get; set; }
    public string? Status { get; set; }
}
