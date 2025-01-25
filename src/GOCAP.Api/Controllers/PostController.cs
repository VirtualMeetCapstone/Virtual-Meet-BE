namespace GOCAP.Api.Controllers;

[Route("posts")]
[ApiController]
public class PostController(IPostService _postService,
    IBlobStorageService _blobStorageService,
    IMapper _mapper) : ApiControllerBase
{

    [HttpGet("{id}")]
    public async Task<PostModel?> GetById([FromRoute] Guid id)
    {
        var result = await _postService.GetByIdAsync(id) ?? throw new ResourceNotFoundException("The post does not exist");
        return _mapper.Map<PostModel>(result);
    }

    [HttpPost]
    public async Task<PostModel> Create([FromForm] PostCreationModel model)
    {
        var medias = new List<Media>();
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            if (model.MediaFiles != null && model.MediaFiles.Count > 0)
            {
                medias = [..(await Task.WhenAll(model.MediaFiles
                        .Where(file => file.Length > 0 && MediaExtension.IsValidMediaExtension(Path.GetExtension(file.FileName).ToLower()))
                        .Select(async file =>
                        {
                            var extension = Path.GetExtension(file.FileName).ToLower();
                            var url = await _blobStorageService.UploadFileAsync(file.OpenReadStream(), ContainerName.ImageContainer, file.FileName);

                            return new Media
                            {
                                Url = url,
                                Type = MediaExtension.GetMediaType(extension)
                            };
                        })))
                ];
            }
            var domain = _mapper.Map<UserPost>(model);
            domain.Medias = medias;
            var post = await _postService.UploadPost(domain);
            var result = _mapper.Map<PostModel>(post);

            transaction.Complete();
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error uploading media files {ex.Message}.");
        }
        finally
        {
            transaction.Dispose();
        }
    }

    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _postService.DeleteByIdAsync(id);
    }
}
