namespace GOCAP.Services.Intention;

public interface IMediaService
{
    Task<List<Media>> UploadMediaFilesAsync(List<MediaUpload> mediaUploads);
}
