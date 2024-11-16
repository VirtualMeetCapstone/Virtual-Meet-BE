using System.ComponentModel.DataAnnotations.Schema;

namespace GOCAP.Database;

[Table("Medias")]
public class MediaEntity : EntityBase
{
    public string Url { get; set; } = string.Empty;
    public MediaType Type { get; set; }
    public string? ThumbnailUrl {  get; set; }

    // Foreign key
    public Guid PostId { get; set; }
}
    