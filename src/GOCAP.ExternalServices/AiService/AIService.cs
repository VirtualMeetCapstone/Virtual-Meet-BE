using System.Text.Json;
using System.Text.RegularExpressions;

namespace GOCAP.ExternalServices
{
    public class AIService : IAIService
    {
        private readonly IModerationRepository _moderationRepository;
        private readonly IAIChatRepository _aiChatRepository;
        private readonly ModerationModel _mode;
        private static readonly Lazy<HashSet<string>> _normalizedBadWords = new(() =>
            LoadBadWordsDict1().Keys.Concat(LoadBadWordsDict2().Keys)
            .Select(w => w.Replace("_", " ").ToLowerInvariant()).ToHashSet());

        public AIService(IModerationRepository moderationRepository, IAIChatRepository aiChatRepository, ModerationModel mode = ModerationModel.RapidAPI)
        {
            _moderationRepository = moderationRepository;
            _aiChatRepository = aiChatRepository;
            _mode = mode;
        }

        public async Task<ModerationResponse> CheckContentAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text không được để trống");

            var matched = GetMatchedBadWords(text);
            if (matched.Any())
                return new ModerationResponse { Status = true, BadWords = matched };

            if (text.Trim().Length < 15 || text.Trim().Length > 150)
                return new ModerationResponse { Status = false };

            try
            {
                return _mode switch
                {
                    ModerationModel.OpenAI => await _moderationRepository.CheckWithOpenAI(text),
                    ModerationModel.RapidAPI => await _moderationRepository.CheckWithRapidAPI(text),
                    _ => throw new NotSupportedException("Chế độ kiểm duyệt không được hỗ trợ")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error in moderation API: {ex.Message}");
                throw;
            }
        }

        public List<string> GetMatchedBadWords(string input)
        {
            var normalized = Regex.Replace(input.ToLowerInvariant(), @"[\p{P}\p{S}_]+", " ").Trim();
            return _normalizedBadWords.Value
                .Where(bw => Regex.IsMatch(normalized, $@"\b{Regex.Escape(bw)}\b"))
                .ToList();
        }

        public async Task<string> SendMessageAsync(string userInput)
        {
            var aiResponse = await _aiChatRepository.SendMessageToAIAsync(userInput);
            return aiResponse;
        }

        private static Dictionary<string, int> LoadBadWordsDict1()
        {
            try
            {
                var json = System.IO.File.ReadAllText("Data/badwords1.json");
                return JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? new();
            }
            catch { return new(); }
        }

        private static Dictionary<string, JsonElement> LoadBadWordsDict2()
        {
            try
            {
                var json = System.IO.File.ReadAllText("Data/badwords2.json");
                return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new();
            }
            catch { return new(); }
        }
    }
}
