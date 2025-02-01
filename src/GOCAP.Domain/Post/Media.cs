namespace GOCAP.Domain;

public class Media : DomainBase
{
    public string Url { get; set; } = string.Empty;
    public MediaType Type { get; set; }
    public string? ThumbnailUrl { get; set; }

    // Foreign key
    public Guid PostId { get; set; }
}
