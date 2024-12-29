namespace GOCAP.Database;

[BsonCollection("ActivityLogs")]
public class ActivityLogEntity : EntityMongoBase
{
    public Guid UserId { get; set; }
    public string? Action { get; set; }
    public DateTime ActionDate { get; set; }
}
