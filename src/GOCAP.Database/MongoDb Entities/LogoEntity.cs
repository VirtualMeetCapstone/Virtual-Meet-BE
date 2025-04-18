namespace GOCAP.Database;

[BsonCollection("Logos")]
public class LogoEntity : EntityMongoBase
{
    public required string Picture { get; set; } 
}

