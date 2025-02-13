namespace GOCAP.Common;

public class Media
{
    public string? Url { get; set; }
    public MediaType Type { get; set; }
    public string? ThumbnailUrl { get; set; } = null;
}

public class MediaUpload
{
    public required Stream FileStream { get; set; }
    public required string FileName { get; set; }
    public string ContainerName { get; set; } = GOCAPConstants.BlobContainer;
    public string? ContentType {  get; set; }
    public long MaxBlobSize { get; set; } = GOCAPConstants.MaxBlobSize;
    public MediaType Type { get; set; }
}