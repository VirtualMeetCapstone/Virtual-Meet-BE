namespace GOCAP.Services;

[RegisterService(typeof(ILogoService))]
public class LogoService(
    IBlobStorageService _blobStorageService,
    ILogoRepository _logoRepository, 
    IMapper _mapper) : ILogoService
{
    public async Task<OperationResult> CreateOrUpdateAsync(Logo domain)
    {
        var logo = await _logoRepository.GetAsync();
        
        if (logo == null)
        {
            domain.InitCreation();
            domain.Media = await _blobStorageService.UploadFileAsync(domain.MediaUpload);
            var entity = _mapper.Map<LogoEntity>(domain);
            await _logoRepository.AddAsync(entity);
            return new OperationResult(true);
        }
        var media = JsonHelper.Deserialize<Media>(logo.Picture);
        var uri = new Uri(media?.Url ?? "");
        string[] segments = uri.AbsolutePath.Trim('/').Split('/');
        if (segments.Length < 2)
        {
            throw new ParameterInvalidException($"Invalid Azure Blob Storage URL format: {logo.Picture}");
        }
        string fileName = segments.Last();
        domain.MediaUpload.FileName = fileName;
        var isPictureUpdated = await _blobStorageService.UpdateFilesAsync([domain.MediaUpload]);
        if (!isPictureUpdated)
        {
            return new OperationResult(isPictureUpdated);
        }
        logo.UpdateModify();
        var result = await _logoRepository.UpdateAsync(logo);
        return new OperationResult(result);
    }

    public async Task<Logo> GetAsync()
    {
        var entity = await _logoRepository.GetAsync();
        return _mapper.Map<Logo>(entity);
    }
}
