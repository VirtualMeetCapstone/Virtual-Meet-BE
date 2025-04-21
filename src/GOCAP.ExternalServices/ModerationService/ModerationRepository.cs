
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace GOCAP.ExternalServices;

public class ModerationRepository : IModerationRepository
{
    private readonly HttpClient _openAIClient;
    private readonly HttpClient _rapidAPIClient;
    private readonly OpenAISettings _openAISettings;
    private readonly string _promptTemplate;

    public ModerationRepository(IHttpClientFactory factory, OpenAISettings openAISettings)
    {
        _openAIClient = factory.CreateClient("OpenAI");
        _rapidAPIClient = factory.CreateClient("RapidAPI");
        _openAISettings = openAISettings;
        _promptTemplate = @"Bạn là hệ thống kiểm duyệt nội dung chuyên nghiệp. Quốc gia chủ yếu của bạn là Việt Nam, 
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
    }

    public async Task<ModerationResponse> CheckWithOpenAI(string text)
    {
        var prompt = new
        {
            model = _openAISettings.OpenAIModel,
            messages = new[]
            {
            new { role = "user", content = string.Format(_promptTemplate, text) }
        },
            temperature = 0.3,
            top_k = 1,
            top_p = 0.7,
            web_access = false
        };

        var content = new StringContent(JsonSerializer.Serialize(prompt), Encoding.UTF8, "application/json");
        var response = await _openAIClient.PostAsync("", content);
        var json = await response.Content.ReadAsStringAsync();

        var doc = JsonDocument.Parse(json);
        var contentResult = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        var match = Regex.Match(contentResult ?? "", "{[^{}]*\"status\"[^{}]*}");
        if (match.Success)
        {
            return JsonSerializer.Deserialize<ModerationResponse>(match.Value)!;
        }

        return new ModerationResponse { Status = false, BadWords = new List<string> { "OpenAI response invalid" } };
    }

    public async Task<ModerationResponse> CheckWithRapidAPI(string text)
    {
        var prompt = new
        {
            messages = new[]
            {
            new { role = "user", content = string.Format(_promptTemplate, text) }
        },
            temperature = 0.3,
            top_k = 1,
            top_p = 0.7
        };

        var content = new StringContent(JsonSerializer.Serialize(prompt), Encoding.UTF8, "application/json");
        var response = await _rapidAPIClient.PostAsync("", content);
        var json = await response.Content.ReadAsStringAsync();

        var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("result", out var resultElement))
        {
            var cleaned = resultElement.GetString()?.Replace("```json", "").Replace("```", "").Trim();
            var parsed = JsonDocument.Parse(cleaned!);
            return JsonSerializer.Deserialize<ModerationResponse>(parsed.RootElement.GetRawText())!;
        }

        return new ModerationResponse { Status = false, BadWords = new List<string> { "API result invalid" } };
    }
}
