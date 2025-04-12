using GOCAP.Database;
namespace GOCAP.Api.Controllers;

[Authorize]
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
        if (User.Identity?.Name != request.UserId.ToString())
        {
            return Forbid("Bạn không có quyền thực hiện hành động này.");
        }
        var result = await _vipPaymentService.CreateVipPaymentAsync(request.UserId,request.PackageId);
        return Ok(result);
    }

    [HttpPost("mark-paid")]
    public async Task<IActionResult> MarkAsPaid([FromQuery] string orderCode)
    {
        var success = await _vipPaymentService.MarkPaymentAsPaidAsync(orderCode);
        if (!success) return NotFound("Không tìm thấy đơn hàng hoặc đã được đánh dấu thanh toán.");
        return Ok("Đã đánh dấu đơn hàng là đã thanh toán.");
    }

}
