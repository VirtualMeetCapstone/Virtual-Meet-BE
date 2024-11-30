namespace GOCAP.Services;

[RegisterService(typeof(IMediaService))]
internal class MediaService(
    IMediaRepository _repository,
    ILogger<MediaService> _logger
    ) : ServiceBase<Media>(_repository, _logger), IMediaService
{
}
