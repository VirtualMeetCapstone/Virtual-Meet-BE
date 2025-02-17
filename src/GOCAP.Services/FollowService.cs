namespace GOCAP.Services;

[RegisterService(typeof(IFollowService))]
internal class FollowService(
    IUnitOfWork _unitOfWork,
    IUserRepository _userRepository,
    IUserNotificationRepository _userNotificationRepository,
    IFollowRepository _repository,
    IMapper _mapper,
    ILogger<FollowService> _logger
    ) : ServiceBase<Follow, UserFollowEntity>(_repository, _mapper, _logger), IFollowService
{
    private readonly IMapper _mapper = _mapper;
    /// <summary>
    /// Follow or unfollow one user.
    /// </summary>
    /// <param name="domain"></param>
    /// <returns>The operation result</returns>
    /// <exception cref="ParameterInvalidException"></exception>
    /// <exception cref="InternalException"></exception>
    public async Task<OperationResult> FollowOrUnfollowAsync(Follow domain)
    {
        if (domain.FollowerId == domain.FollowingId)
        {
            throw new ParameterInvalidException();
        }

        if (!await _userRepository.CheckExistAsync(domain.FollowerId))
        {
            throw new ResourceNotFoundException($"User {domain.FollowerId} was not found.");
        }

        if (!await _userRepository.CheckExistAsync(domain.FollowingId))
        {
            throw new ResourceNotFoundException($"User {domain.FollowingId} was not found.");
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
                var sender = await _userRepository.GetByIdAsync(domain.FollowerId);
                
                // Begin the transaction. 
                await _unitOfWork.BeginTransactionAsync();

                // Insert the follow into db.
                domain.InitCreation();
                var entity = _mapper.Map<UserFollowEntity>(domain);
                await _repository.AddAsync(entity);

                // Insert the notification into db.
                var notification = new UserNotification
                {
                    UserId = domain.FollowingId,
                    Sender = _mapper.Map<User>(sender),
                    Content = string.Format(NotificationMessage.Follow, sender.Name),
                    Type = NotificationType.Follow,
                    ReferenceId = domain.FollowerId,
                };
                notification.InitCreation();
                var notificationEntity = _mapper.Map<UserNotificationEntity>(notification);
                await _userNotificationRepository.AddAsync(notificationEntity);

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
