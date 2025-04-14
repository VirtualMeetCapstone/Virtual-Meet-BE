namespace GOCAP.Api.Controllers;

[Route("admin")]
public class LogoController(IMapper _mapper, ILogoAdminService _logoService) : ApiControllerBase
{
    [HttpPut("logo")]
    public async Task<IActionResult> UpdateLogo([FromForm] LogoUpdateModel model)
    {
        var domain = _mapper.Map<LogoUpdate>(model);
        var updated = await _logoService.UpdateAsync(domain);
        var result = _mapper.Map<LogoUpdateModel>(updated);
        return Ok(result);
    }
}
