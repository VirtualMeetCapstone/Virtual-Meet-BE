
namespace GOCAP.ExternalServices
{
    public interface IModerationRepository
    {
        Task<ModerationResponse> CheckWithOpenAI(string text);
        Task<ModerationResponse> CheckWithRapidAPI(string text);
    }
}
