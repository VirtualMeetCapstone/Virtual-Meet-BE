namespace GOCAP.Api.Controllers;

[Route("ytb")]
public class YoutubeController(IHttpClientFactory httpClientFactory, YoutubeSettings settings) : ApiControllerBase
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    private readonly YoutubeSettings _settings = settings;

    [HttpGet("trending")]
    public async Task<IActionResult> GetTrendingVideos([FromQuery] string pageToken = "")
    {
        var url = $"{_settings.API_URL}/videos?part=snippet&chart=mostPopular&regionCode=VN&maxResults=10&key={_settings.API_KEY}";

        if (!string.IsNullOrEmpty(pageToken))
        {
            url += $"&pageToken={pageToken}";
        }

        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        return Content(content, "application/json");
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchVideos([FromQuery] string q)
    {
        var url = $"{_settings.API_URL}/search?part=snippet&q={q}&type=video&maxResults=10&key={_settings.API_KEY}";

        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        return Content(content, "application/json");
    }
}
