using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GOCAP.Api.Controllers;

[Route("moderation")]
public class ModerationController : ApiControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ModerationSettings _settings;
    private readonly OpenAISettings _openAISettings;
    private const ModerationModel CURRENT_MODEL = ModerationModel.RapidAPI;
    private const ModerationModel CURRENT_MODEL_FOR_CHAT = ModerationModel.RapidAPI;

    private const string MODERATION_PROMPT_TEMPLATE = @"Bạn là hệ thống kiểm duyệt nội dung chuyên nghiệp. Quốc gia chủ yếu của bạn là Việt Nam, 
                                vì vậy bạn cần nhận biết đúng các nội dung nhạy cảm đặc thù trong văn hóa và chính trị Việt Nam.
                                Hãy phân tích đoạn văn dưới đây (tiếng Việt hoặc tiếng Anh) và trả lời duy nhất ở định dạng JSON ((KHÔNG được thêm giải thích, không được dùng ```json hoặc bất kỳ ký tự markdown nào)):
                                {{""status"": true}} nếu nội dung vi phạm
                                {{""status"": false}} nếu nội dung phù hợp
                                Trả về true nếu đoạn văn có nội dung không phù hợp như:
                                - 18+, khiêu dâm, tục tĩu
                                - Bạo lực, tự tử
                                - Chính trị nhạy cảm, xúc phạm lãnh tụ hoặc các lãnh tụ chính trị, hoặc gây hiểu lầm về chính sách quốc gia Việt Nam.
                                - Tin giả, lừa đảo

                                Đoạn văn: ""{0}""";

    private enum ModerationModel
    {
        OpenAI,
        RapidAPI,
    }

    // Singleton pattern for thread-safe lazy initialization
    private static readonly Lazy<Dictionary<string, int>> _badWords1 = new Lazy<Dictionary<string, int>>(() => LoadBadWordsDict1());
    private static readonly Lazy<Dictionary<string, JsonElement>> _badWords2 = new Lazy<Dictionary<string, JsonElement>>(() => LoadBadWordsDict2());

    // HashSet for faster lookups
    private static readonly Lazy<HashSet<string>> _normalizedBadWords = new Lazy<HashSet<string>>(() =>
        _badWords1.Value.Keys.Concat(_badWords2.Value.Keys)
            .Select(word => word.Replace("_", " ").ToLowerInvariant())
            .ToHashSet());

    public ModerationController(ModerationSettings settings, IHttpClientFactory factory, OpenAISettings openAISettings)
    {
        _settings = settings;
        _httpClient = factory.CreateClient();
        _openAISettings = openAISettings;
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
            return CURRENT_MODEL switch
            {
                ModerationModel.OpenAI => await CheckWithOpenAI(request.Text),
                ModerationModel.RapidAPI => await CheckWithRapidAPI(request.Text),
                _ => throw new NotSupportedException("Unsupported moderation model")
            };
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

    private async Task<ActionResult<ModerationResponse>> CheckWithOpenAI(string text)
    {
        var prompt = new
        {
            model = _openAISettings.OpenAIModel,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = string.Format(MODERATION_PROMPT_TEMPLATE, text)
                }
            },
            temperature = 0.3,
            top_k = 1,
            top_p = 0.7,

            web_access = false
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _openAISettings.OpenAIUrl)
        {
            Content = new StringContent(JsonSerializer.Serialize(prompt), Encoding.UTF8, "application/json")
        };

        httpRequest.Headers.Add("Authorization", $"Bearer {_openAISettings.OpenAIKey}");

        var response = await _httpClient.SendAsync(httpRequest);
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var content = doc.RootElement
                         .GetProperty("choices")[0]
                         .GetProperty("message")
                         .GetProperty("content")
                         .GetString();

        var match = Regex.Match(content ?? "", "{[^{}]*\"status\"[^{}]*}");
        if (match.Success)
        {
            var result = JsonSerializer.Deserialize<ModerationResponse>(match.Value);
            return result!;
        }

        return BadRequest(new { error = "Không đọc được phản hồi từ OpenAI API" });
    }

    private async Task<ActionResult<ModerationResponse>> CheckWithRapidAPI(string text)
    {
        var prompt = new
        {
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = string.Format(MODERATION_PROMPT_TEMPLATE, text)
                }
            },
            temperature = 0.3,
            top_k = 1,
            top_p = 0.7,

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
                    .Replace("```", "")      // Xóa phần kết thúc markdown
                    .Trim();                // Loại bỏ khoảng trắng thừa

                // Giải mã chuỗi JSON đã giải thoát
                try
                {
                    using var resultDoc = JsonDocument.Parse(cleanedResultString);
                    var resultJson = resultDoc.RootElement.GetRawText();

                    // Tiến hành deserialize phần result
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var finalResult = JsonSerializer.Deserialize<ModerationResponse>(resultJson, options);

                    if (finalResult?.BadWords == null)
                    {
                        finalResult!.BadWords = new List<string> { "invalid" };
                    }
                    return finalResult!;
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

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessageUnified([FromBody] UserMessage request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest("Message content is empty.");

        var systemPrompt = "Bạn là một trợ lý AI cho trang web của chúng tôi. Trang web này chuyên về một hệ thống phòng họp ảo có tên là VirtualMeet. VirtualMeet là nền tảng giao tiếp toàn cầu, hỗ trợ họp video, ghi hình, chia sẻ màn hình, phụ đề tự động và phiên dịch đa ngôn ngữ. Hãy hỗ trợ người dùng khi họ có câu hỏi.";

        var messages = new[]
        {
        new { role = "system", content = systemPrompt },
        new { role = "user", content = request.Text }
    };

        try
        {
            HttpRequestMessage httpRequest;

            if (CURRENT_MODEL_FOR_CHAT == ModerationModel.OpenAI)
            {
                var openAIPayload = new
                {
                    messages,
                    temperature = 1,
                    top_p = 1,
                    model = _openAISettings.OpenAIModel
                };

                httpRequest = new HttpRequestMessage(HttpMethod.Post, _openAISettings.OpenAIUrl)
                {
                    Content = new StringContent(JsonSerializer.Serialize(openAIPayload), Encoding.UTF8, "application/json")
                };
                httpRequest.Headers.Add("Authorization", $"Bearer {_openAISettings.OpenAIKey}");
            }
            else // RapidAPI
            {
                var rapidPayload = new
                {
                    bot_id = "OEXJ8qFp5E5AwRwymfPts90vrHnmr8yZgNE171101852010w2S0bCtN3THp448W7kDSfyTf3OpW5TUVefz",
                    messages,
                    user_id = "",
                    temperature = 0.3,
                    top_k = 1,
                    top_p = 0.7,
                    max_tokens = 256,
                    model = "gpt 3.5"
                };

                httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://open-ai21.p.rapidapi.com/chatbotapi")
                {
                    Content = new StringContent(JsonSerializer.Serialize(rapidPayload), Encoding.UTF8, "application/json")
                };
                httpRequest.Headers.Add("X-RapidAPI-Key", _settings.ApiKey);
                httpRequest.Headers.Add("X-RapidAPI-Host", _settings.ApiHost);
            }

            var response = await _httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { message = error });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);

            string aiText;
            if (CURRENT_MODEL_FOR_CHAT == ModerationModel.OpenAI)
            {
                aiText = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
            }
            else
            {
                aiText = doc.RootElement.GetProperty("result").GetString();
            }

            return Ok(new { text = aiText ?? "No response from AI" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error communicating with AI", error = ex.Message });
        }
    }


    public class UserMessage
    {
        public string Text { get; set; }
    }

}

