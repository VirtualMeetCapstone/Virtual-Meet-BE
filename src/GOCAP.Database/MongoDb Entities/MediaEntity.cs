namespace GOCAP.Database;

[BsonCollection("Medias")]
public class MediaEntity : EntityMongoBase
{
    public string Url { get; set; } = string.Empty;
    public MediaType Type { get; set; }
    public string? ThumbnailUrl { get; set; } = null;
}
