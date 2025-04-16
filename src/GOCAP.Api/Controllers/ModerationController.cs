using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GOCAP.Api.Controllers;

[Route("moderation")]
public class ModerationController : ApiControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ModerationSettings _settings;

    // Singleton pattern for thread-safe lazy initialization
    private static readonly Lazy<Dictionary<string, int>> _badWords1 = new Lazy<Dictionary<string, int>>(() => LoadBadWordsDict1());
    private static readonly Lazy<Dictionary<string, JsonElement>> _badWords2 = new Lazy<Dictionary<string, JsonElement>>(() => LoadBadWordsDict2());

    // HashSet for faster lookups
    private static readonly Lazy<HashSet<string>> _normalizedBadWords = new Lazy<HashSet<string>>(() =>
        _badWords1.Value.Keys.Concat(_badWords2.Value.Keys)
            .Select(word => word.Replace("_", " ").ToLowerInvariant())
            .ToHashSet());

    public ModerationController(ModerationSettings settings, IHttpClientFactory factory)
    {
        _settings = settings;
        _httpClient = factory.CreateClient();
    }

    private static Dictionary<string, int> LoadBadWordsDict1()
    {
        try
        {
            var json = System.IO.File.ReadAllText("Data/badwords1.json");
            return JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? new Dictionary<string, int>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Failed to load badwords1.json: " + ex.Message);
            return new Dictionary<string, int>();
        }
    }

    private static Dictionary<string, JsonElement> LoadBadWordsDict2()
    {
        try
        {
            var json = System.IO.File.ReadAllText("Data/badwords2.json");
            return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new Dictionary<string, JsonElement>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Failed to load badwords2.json: " + ex.Message);
            return new Dictionary<string, JsonElement>();
        }
    }

    private List<string> GetMatchedBadWords(string input)
    {
        var normalizedInput = NormalizeText(input);

        var matched = _normalizedBadWords.Value
            .Where(badWord => Regex.IsMatch(normalizedInput, $@"\b{Regex.Escape(badWord)}\b"))
            .ToList();

        return matched;
    }

    private string NormalizeText(string input)
    {
        return Regex.Replace(input.ToLowerInvariant(), @"[\p{P}\p{S}_]+", " ").Trim();
    }

    [HttpPost("check")]
    public async Task<ActionResult<ModerationResponse>> CheckContent([FromBody] ModerationRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest(new { error = "Text không được để trống" });
        }

        var matchedBadWords = GetMatchedBadWords(request.Text);

        if (matchedBadWords.Any())
        {
            return Ok(new ModerationResponse
            {
                Status = true,
                BadWords = matchedBadWords
            });
        }

        var textLength = request.Text.Trim().Length;
        if (textLength < 15 || textLength > 150)
        {
            return Ok(new ModerationResponse
            {
                Status = false,
                BadWords = null
            });
        }

        try
        {
            var prompt = new
            {
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = @$"Bạn là hệ thống kiểm duyệt nội dung chuyên nghiệp. Quốc gia chủ yếu của bạn là Việt Nam, 
                                    vì vậy bạn cần nhận biết đúng các nội dung nhạy cảm đặc thù trong văn hóa và chính trị Việt Nam.
                                    Hãy phân tích đoạn văn dưới đây (tiếng Việt hoặc tiếng Anh) và trả lời duy nhất ở định dạng JSON:

                                    {{""status"": true | false}}

                                    Trả về true nếu đoạn văn có nội dung không phù hợp như:
                                    - 18+, khiêu dâm, tục tĩu
                                    - Bạo lực, tự tử
                                    - Chính trị nhạy cảm, xúc phạm lãnh tụ hoặc các lãnh tụ chính trị, hoặc gây hiểu lầm về chính sách quốc gia
                                    - Tin giả, lừa đảo

                                    Đoạn văn: ""{request.Text}""
"
                    }
                },
                web_access = false
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _settings.ApiUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(prompt), Encoding.UTF8, "application/json")
            };

            httpRequest.Headers.Add("X-RapidAPI-Key", _settings.ApiKey);
            httpRequest.Headers.Add("X-RapidAPI-Host", _settings.ApiHost);

            using var response = await _httpClient.SendAsync(httpRequest);
            var json = await response.Content.ReadAsStringAsync();

            // Giải mã JSON chứa kết quả từ "result"
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("result", out var resultElement) && resultElement.ValueKind == JsonValueKind.String)
            {
                var resultString = resultElement.GetString();
                if (!string.IsNullOrEmpty(resultString))
                {
                    // Loại bỏ phần markdown (```` ```json ... ``` ```)
                    var cleanedResultString = resultString
                        .Replace("```json", "") 
                        .Replace("```", "")      
                        .Trim();                
                    try
                    {
                        using var resultDoc = JsonDocument.Parse(cleanedResultString);
                        var resultJson = resultDoc.RootElement.GetRawText();

                        // Tiến hành deserialize phần result
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var finalResult = JsonSerializer.Deserialize<ModerationResponse>(resultJson, options);
                     
                        if (finalResult.BadWords == null)
                        {
                            finalResult.BadWords = new List<string> { "invalid" };
                        }
                        return Ok(finalResult);
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"⚠️ Error while parsing result JSON: {ex.Message}");
                        return BadRequest(new { error = "Lỗi phân tích dữ liệu JSON trong kết quả" });
                    }
                }
            }

            return BadRequest(new { error = "Không phân tích được kết quả từ API" });
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"⚠️ API request error: {ex.Message}");
            return StatusCode(503, new { error = "Lỗi kết nối đến API kiểm duyệt" });
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"⚠️ JSON parsing error: {ex.Message}");
            return BadRequest(new { error = "Lỗi phân tích phản hồi từ API" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Unexpected error: {ex.Message}");
            return StatusCode(500, new { error = "Lỗi hệ thống khi kiểm duyệt nội dung" });
        }
    }
}