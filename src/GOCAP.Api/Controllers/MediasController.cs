namespace GOCAP.Api.Controllers;

[Route("medias")]
public class MediasController(IMediaService _service) : ApiControllerBase
{
    [HttpPost]
    public async Task<List<Media>> UploadMediaFiles([FromForm] List<IFormFile> medias)
    {
        var result = await _service.UploadMediaFilesAsync(ConvertMediaHelper.ConvertFormFilesToMedias(medias));
        return result;
    }

    [HttpDelete]
    public async Task<OperationResult> DeleteMediaFiles([FromQuery] List<string> urls)
    {
        var result = await _service.DeleteMediaFilesAsync(urls);
        return result;
    }
}