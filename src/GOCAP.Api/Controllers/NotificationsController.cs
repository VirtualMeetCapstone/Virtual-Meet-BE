namespace GOCAP.Api.Controllers;

[ApiController]
public class NotificationsController (INotificationService _service, IMapper _mapper) : ApiControllerBase
{
    /// <summary>
    /// Get notifications by user id.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("users/{userId}/notifications")]
    public async Task<QueryResult<NotificationModel>> GetNotificationsByUserId([FromRoute] Guid userId, [FromQuery] QueryInfo queryInfo)
    {
        var result = await _service.GetNotificationsByUserIdAsync(userId, queryInfo);
        return _mapper.Map<QueryResult<NotificationModel>>(result);
    }

    [HttpDelete("notifications/{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        var result = await _service.DeleteByIdAsync(id);
        return result;
    }

    [HttpPatch("users/{userId}/notifications/{notificationId}/mark-as-read")]
    public async Task<OperationResult> MarkAsRead([FromRoute] Guid userId, [FromRoute] Guid notificationId)
    {
        var result = await _service.MarkAsReadAsync(userId, notificationId);
        return result;
    }
}
