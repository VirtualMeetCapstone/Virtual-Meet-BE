namespace GOCAP.Api.Controllers;

[Route("admin")]
public class LogosController(IMapper _mapper, ILogoService _logoService) : ApiControllerBase
{
    [HttpGet("logo")]
    public async Task<LogoModel> GetLogo()
    {
        var domain = await _logoService.GetAsync();
        var result = _mapper.Map<LogoModel>(domain);
        return result;
    }

    [HttpPut("logo")]
    public async Task<OperationResult> CreateOrUpdateLogo([FromForm] LogoModel model)
    {
        var domain = _mapper.Map<Logo>(model);
        var result = await _logoService.CreateOrUpdateAsync(domain);
        return result;
    }
}
