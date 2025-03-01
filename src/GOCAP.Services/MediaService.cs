namespace GOCAP.Services;

[RegisterService(typeof(IMediaService))]
internal class MediaService (ILogger<MediaService> _logger, IBlobStorageService _blobStorageService) : IMediaService
{
    public async Task<List<Media>> UploadMediaFilesAsync(List<MediaUpload> mediaUploads)
    {
        try
        {
            _logger.LogInformation("Start upload an media of type {EntityType}.", typeof(MediaUpload).Name);
            if (mediaUploads.Count == 0)
            {
                throw new ParameterInvalidException("Media upload must be not null.");
            }
            var medias = await _blobStorageService.UploadFilesAsync(mediaUploads);
            return medias;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected errors occur while updating room");
            if (mediaUploads.Count > 0)
            {
                await MediaHelper.DeleteMediaFilesIfError(mediaUploads, _blobStorageService);
            }
            throw new InternalException();
        }
    }

    public async Task<OperationResult> DeleteMediaFilesAsync(List<string> mediaUrls)
    {
        if (mediaUrls.Count == 0 || mediaUrls == null)
        {
            throw new ParameterInvalidException();
        }
        var result = await _blobStorageService.DeleteFilesByUrlsAsync(mediaUrls);
        return new OperationResult(result);
    }
}
