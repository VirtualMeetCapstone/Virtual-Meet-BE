namespace GOCAP.Api.Controllers;

[Route("moderation")]
public class ModerationController : ApiControllerBase
{
    private readonly IAIService _aIService;

    public ModerationController(IAIService moderationService)
    {
        _aIService = moderationService;
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckContent([FromBody] ModerationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Vui lòng nhập nội dung để kiểm duyệt." });

        var result = await _aIService.CheckContentAsync(request.Text);
        return Ok(result);
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] ModerationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Vui lòng nhập tin nhắn." });

        var response = await _aIService.SendMessageAsync(request.Text);
        return Ok(new { text = response });
    }

    public class UserMessage
    {
        public string Text { get; set; }
    }
}

