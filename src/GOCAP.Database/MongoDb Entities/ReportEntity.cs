namespace GOCAP.Database;
using MongoDB.Bson;
[BsonCollection("Reports")]
   public class ReportEntity : EntityMongoBase
{
    [BsonRepresentation(BsonType.String)]
    public Guid TargetId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid ReporterId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public ReportTypeEnum ReportType { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }
}


