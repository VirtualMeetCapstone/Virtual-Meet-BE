namespace GOCAP.ExternalServices;

public interface IAIChatRepository
{
    Task<string> SendMessageToAIAsync(string userInput);
    Task<string> AISummaryAsync(string fullText);
}
