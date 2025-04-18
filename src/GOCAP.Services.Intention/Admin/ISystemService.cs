
namespace GOCAP.Services.Intention;

public interface ISystemService
{
    Task<OperationResult> SendSystemNotificationToAllUsersAsync(string subject, string content);
}
