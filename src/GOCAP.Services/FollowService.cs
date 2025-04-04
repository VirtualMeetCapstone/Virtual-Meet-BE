namespace GOCAP.Services;

[RegisterService(typeof(IFollowService))]
internal class FollowService(
    IUserContextService _userContextService,
    IUserRepository _userRepository,
    IFollowRepository _repository,
    IKafkaProducer _kafkaProducer,
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

        var result = new OperationResult(true);
        var follow = await _repository.GetByFollowerAndFollowingAsync(_userContextService.Id, domain.FollowingId);

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
            domain.InitCreation();
            var entity = _mapper.Map<UserFollowEntity>(domain);
            entity.FollowerId = _userContextService.Id;
            await _repository.AddAsync(entity);
            await _kafkaProducer.ProduceAsync(KafkaConstants.Topics.Notification, new NotificationEvent
            {
                Type = NotificationType.Follow,
                ActionType = ActionType.Add,
                UserId = entity.FollowingId,
                ActorId = entity.FollowerId
            });
        }
        return result;
    }

    public async Task<bool> IsFollowingAsync(Guid followingId)
    {
        var domain = new Follow
        {
            FollowerId = _userContextService.Id,
            FollowingId = followingId
        };
        return await _repository.IsFollowingAsync(domain);
    }
}
