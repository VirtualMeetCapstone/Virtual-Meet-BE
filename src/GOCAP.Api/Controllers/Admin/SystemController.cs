namespace GOCAP.Api.Controllers;

public class SystemController(ISystemService _service) : ApiControllerBase
{
    [HttpPost("notifications/system")]
    public async Task<OperationResult> SendSystemNotificationToAllUsers([FromBody] SystemNotificationModel model)
    {
        var result = await _service.SendSystemNotificationToAllUsersAsync(model.Subject, model.Content);
        return result;
    }
}
