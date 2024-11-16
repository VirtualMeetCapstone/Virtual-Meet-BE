namespace GOCAP.Api.Validation;

public abstract class ValidatorBase<T> : AbstractValidator<T>
{
    private static readonly HashSet<string> ValidImageExtensions =
    [
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff"
    ];

    private static readonly HashSet<string> ValidVideoExtensions =
    [
        ".mp4", ".avi", ".mov", ".wmv", ".mkv"
    ];

    protected static bool ValidateMediaCount(ICollection<IFormFile>? mediaFiles)
    {
        if (mediaFiles == null || mediaFiles.Count == 0)
        {
            return false;
        }

        int imageCount = 0;
        int videoCount = 0;

        foreach (var file in mediaFiles)
        {
            // Get tail file
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (ValidImageExtensions.Contains(extension))
            {
                imageCount++;
            }
            else if (ValidVideoExtensions.Contains(extension))
            {
                videoCount++;
            }
            else
            {
                return false;
            }
        }

        return (imageCount >= 1 && imageCount <= 5 && videoCount == 0) || (videoCount == 1 && imageCount == 0);
    }
}
