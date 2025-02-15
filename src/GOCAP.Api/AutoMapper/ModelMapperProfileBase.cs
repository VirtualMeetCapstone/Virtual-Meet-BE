namespace GOCAP.Api.AutoMapper;

public abstract class ModelMapperProfileBase : Profile
{
    protected static List<IFormFile>? ConvertMediasToFormFiles(List<MediaUpload>? medias)
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

    protected static List<MediaUpload> ConvertFormFilesToMedias(List<IFormFile>? formFiles)
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

    protected static IFormFile? ConvertMediaToFormFile(MediaUpload? media)
    {
        if (media?.FileStream == null)
            return null;

        return new FormFile(media.FileStream, 0, media.FileStream.Length, "file", media.FileName ?? string.Empty);
    }

    protected static MediaUpload? ConvertFormFileToMedia(IFormFile? formFile)
    {
        if (formFile == null)
            return null;

        return new MediaUpload
        {
            FileName = formFile.FileName,
            FileStream = formFile.OpenReadStream(),
            Type = GetMediaType(formFile.ContentType),
        };
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
