namespace GOCAP.Common;

public class MediaExtension
{
    public static readonly List<string> FileExtensions =
    [
        ".jpg", ".jpeg", ".png",
        ".gif", ".bmp", ".tiff",
        ".svg", ".mp4", ".avi",
        ".mov", ".wmv", ".mkv",
        ".flv", ".webm"
    ];

    // Check if a file extension is valid for media
    public static bool IsValidMediaExtension(string extension)
    {
        return FileExtensions.Contains(extension.ToLower());
    }

    // Get media type based on extension (Image or Video)
    public static MediaType GetMediaType(string extension)
    {
        return extension.StartsWith(".mp4") || extension.StartsWith(".avi") ||
               extension.StartsWith(".mov") || extension.StartsWith(".wmv") ||
               extension.StartsWith(".mkv") || extension.StartsWith(".flv") ||
               extension.StartsWith(".webm")
            ? MediaType.Video
            : MediaType.Image;
    }
}
