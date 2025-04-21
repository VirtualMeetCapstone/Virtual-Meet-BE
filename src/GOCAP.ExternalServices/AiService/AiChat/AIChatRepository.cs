using GOCAP.Repository.Intention;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GOCAP.ExternalServices
{
    [RegisterService(typeof(IAIChatRepository))]
    public class AIChatRepository : IAIChatRepository
    {
        private readonly HttpClient _openAIClient;
        private readonly HttpClient _rapidAPIClient;
        private readonly OpenAISettings _openAISettings;
        private const ModerationModel CURRENT_MODEL = ModerationModel.RapidAPI;

        public AIChatRepository(IHttpClientFactory factory, OpenAISettings openAISettings)
        {
            _openAIClient = factory.CreateClient("OpenAI");
            _rapidAPIClient = factory.CreateClient("RapidAPI");
            _openAISettings = openAISettings ?? throw new ArgumentNullException(nameof(openAISettings));
        }

        public async Task<string> SendMessageToAIAsync(string userInput)
        {
            // Tạo hệ thống prompt cho trợ lý AI
            var systemPrompt = "Bạn là một trợ lý AI cho trang web của chúng tôi. Trang web này chuyên về một hệ thống phòng họp ảo có tên là VirtualMeet. VirtualMeet là nền tảng giao tiếp toàn cầu, hỗ trợ họp video, ghi hình, chia sẻ màn hình, phụ đề tự động và phiên dịch đa ngôn ngữ. Hãy hỗ trợ người dùng khi họ có câu hỏi.";

            var messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userInput }
            };

            HttpResponseMessage response;
            string responseBody;

            // Gửi yêu cầu đến OpenAI hoặc RapidAPI tùy theo chế độ
            if (CURRENT_MODEL == ModerationModel.OpenAI)
            {
                var openAIPayload = new
                {
                    messages,
                    temperature = 1,
                    top_p = 1,
                    model = _openAISettings.OpenAIModel
                };

                var content = new StringContent(JsonSerializer.Serialize(openAIPayload), Encoding.UTF8, "application/json");
                response = await _openAIClient.PostAsync("", content);
            }
            else
            {
                var rapidPayload = new
                {
                    bot_id = "OEXJ8qFp5E5AwRwymfPts90vrHnmr8yZgNE171101852010w2S0bCtN3THp448W7kDSfyTf3OpW5TUVefz",
                    messages,
                    temperature = 0.3,
                    top_k = 1,
                    top_p = 0.7,
                    max_tokens = 256,
                    model = "gpt-3.5"
                };

                var content = new StringContent(JsonSerializer.Serialize(rapidPayload), Encoding.UTF8, "application/json");
                response = await _rapidAPIClient.PostAsync("", content);
            }

            // Kiểm tra xem phản hồi có thành công không
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {error}");
            }

            // Đọc và xử lý phản hồi
            responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);

            // Trích xuất phản hồi từ AI dựa trên mô hình sử dụng
            string aiResponse = CURRENT_MODEL switch
            {
                ModerationModel.OpenAI => doc.RootElement
                                               .GetProperty("choices")[0]
                                               .GetProperty("message")
                                               .GetProperty("content")
                                               .GetString(),
                _ => doc.RootElement.GetProperty("result").GetString()
            };

            // Nếu không có phản hồi từ AI, trả về một thông báo mặc định
            aiResponse = aiResponse ?? "Không có phản hồi từ AI.";

            return aiResponse;
        }
    }



}
