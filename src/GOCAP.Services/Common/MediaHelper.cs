namespace GOCAP.Services;

public static class MediaHelper
{
    public static async Task DeleteMediaFilesIfError(List<MediaUpload>? mediaUploads, IBlobStorageService blobStorageService)
    {
        if (mediaUploads is null || mediaUploads.Count == 0)
            return; 

        var mediaDeletes = mediaUploads
            .Select(m => new MediaDelete
            {
                ContainerName = m.ContainerName,
                FileName = m.FileName
            })
            .ToList();

        await blobStorageService.DeleteFilesAsync(mediaDeletes);
    }

}
