namespace GOCAP.Api.AutoMapper;

public abstract class ModelMapperProfileBase : Profile
{
    protected static List<IFormFile>? ConvertMediaToFormFiles(List<MediaUpload>? medias)
    {
        if (medias == null || medias.Count == 0)
            return null;

        var formFiles = new List<IFormFile>();

        foreach (var media in medias)
        {
            var stream = media.FileStream;
            var fileName = media.FileName ?? string.Empty;

            if (stream != null)
            {
                var formFile = new FormFile(stream, 0, stream.Length, "file", fileName);
                formFiles.Add(formFile);
            }
        }

        return formFiles;
    }

    protected static List<MediaUpload> ConvertFormFilesToMedia(List<IFormFile>? formFiles)
    {
        if (formFiles == null || formFiles.Count == 0)
            return [];

        return formFiles.Select(file => new MediaUpload
        {
            FileName = file.FileName,
            FileStream = file.OpenReadStream(),
            Type = GetMediaType(file.ContentType),
        }).ToList();
    }

    protected static MediaType GetMediaType(string contentType)
    {
        if (contentType.StartsWith("image/"))
            return MediaType.Image;
        if (contentType.StartsWith("video/"))
            return MediaType.Video;
        return MediaType.Text;
    }
}
