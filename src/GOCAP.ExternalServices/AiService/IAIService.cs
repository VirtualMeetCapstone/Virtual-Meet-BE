
namespace GOCAP.ExternalServices
{
    public interface IAIService
    {
        Task<ModerationResponse> CheckContentAsync(string text);
        Task<string> SendMessageAsync(string userInput);
    }

}
