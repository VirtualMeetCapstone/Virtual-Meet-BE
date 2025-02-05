namespace GOCAP.Services;

[RegisterService(typeof(IFollowService))]
internal class FollowService(
    IUnitOfWork _unitOfWork,
    IUserRepository _userRepository,
    IUserNotificationRepository _userNotificationRepository,
    IFollowRepository _repository,
    ILogger<FollowService> _logger
    ) : ServiceBase<Follow>(_repository, _logger), IFollowService
{
    /// <summary>
    /// Follow or unfollow one user.
    /// </summary>
    /// <param name="domain"></param>
    /// <returns>The operation result</returns>
    /// <exception cref="ParameterInvalidException"></exception>
    /// <exception cref="InternalException"></exception>
    public async Task<OperationResult> FollowAsync(Follow domain)
    {
        if (domain.FollowerId == domain.FollowingId)
        {
            throw new ParameterInvalidException();
        }

        var result = new OperationResult(true);

        var follow = await _repository.GetByFollowerAndFollowingAsync(domain.FollowerId, domain.FollowingId);

        // If existing then remove the follow ( unfollow )
        if (follow != null)
        {
            _logger.LogInformation("Start deleting an entity of type {EntityType}.", typeof(Follow).Name);
            var resultDelete = await _repository.DeleteByIdAsync(follow.Id);
            result = new OperationResult(resultDelete > 0);
        }
        // If not existing then add the follow ( follow )
        else
        {
            _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Follow).Name);
            try
            {
                // Begin the transaction. 
                await _unitOfWork.BeginTransactionAsync();

                // Insert the follow into db.
                domain.InitCreation();
                await _repository.AddAsync(domain);

                // Insert the notification into db.
                var sender = await _userRepository.GetByIdAsync(domain.FollowerId);
                var notification = new UserNotification
                {
                    UserId = domain.FollowingId,
                    Sender = sender,
                    Content = string.Format(NotificationMessage.Follow, sender.Name),
                    Type = NotificationType.Follow,
                    ReferenceId = domain.FollowerId,
                };
                notification.InitCreation();
                await _userNotificationRepository.AddAsync(notification);

                // Complete the transaction.
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting entity of type {EntityType}.", typeof(Follow).Name);
                // Rollback the transaction if occuring the exception.
                await _unitOfWork.RollbackTransactionAsync();
                throw new InternalException();
            };
        }

        return result;
    }

}
