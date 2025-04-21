namespace GOCAP.ExternalServices;

public interface IAIChatRepository
{
    Task<string> SendMessageToAIAsync(string userInput);
}
