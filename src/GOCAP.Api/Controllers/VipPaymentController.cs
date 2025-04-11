using GOCAP.Database;
namespace GOCAP.Api.Controllers;

[Route("/vip-payment")]
[ApiController]
public class VipPaymentController : ControllerBase
{
    private readonly IVipPaymentService _vipPaymentService;

    public VipPaymentController(IVipPaymentService vipPaymentService)
    {
        _vipPaymentService = vipPaymentService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateVipPayment([FromBody] PaymentRequest request)
    {
        var result = await _vipPaymentService.CreateVipPaymentAsync(request.PackageId);
        return Ok(result);
    }
}
