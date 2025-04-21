namespace GOCAP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubtitleController : ControllerBase
    {
        private readonly IRedisService _redisService;

        public SubtitleController(IRedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpGet("room-subtitles/{roomId}")]
        public async Task<IActionResult> GetRoomSubtitles(string roomId)
        {
            var key = $"room:{roomId}:subtitles";
            var subtitles = await _redisService.GetAllHashFieldsAsync(key);
            return Ok(subtitles);
        }
    }
}
