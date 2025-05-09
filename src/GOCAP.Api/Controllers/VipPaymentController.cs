﻿using GOCAP.Database;
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

    [HttpGet("all")]
    public async Task<ActionResult<QueryResult<PaymentHistoryModel>>> GetAllPayments([FromQuery] QueryInfo queryInfo)
    {
        var result = await _vipPaymentService.GetAllPaymentsAsync(queryInfo);
        return Ok(result);
    }


    [HttpGet("payment-statistics")]
    public async Task<ActionResult<QueryResult<PaymentStatisticModel>>> GetPaymentStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        if (startDate == null || endDate == null)
        {
            return BadRequest(new { Message = "StartDate and EndDate are required." });
        }

        var statistics = await _vipPaymentService.GetPaymentStatisticsAsync(startDate.Value, endDate.Value);

        return Ok(statistics);
    }


    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPaymentsByUserId([FromRoute] Guid userId, [FromQuery] QueryInfo queryInfo)
    {
        var result = await _vipPaymentService.GetPaymentsByUserIdAsync(userId, queryInfo);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateVipPayment([FromBody] PaymentRequest request)
    {
        if (User.Identity?.Name != request.UserId.ToString())
        {
            return Forbid("Bạn không có quyền thực hiện hành động này.");
        }
        var result = await _vipPaymentService.CreateVipPaymentAsync(request.UserId, request.PackageId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("mark-paid")]
    public async Task<IActionResult> MarkAsPaid([FromQuery] string orderCode)
    {
        var success = await _vipPaymentService.MarkPaymentAsPaidAsync(orderCode);
        if (!success) return NotFound("Không tìm thấy đơn hàng hoặc đã được đánh dấu thanh toán.");
        return Ok("Đã đánh dấu đơn hàng là đã thanh toán.");
    }

}
