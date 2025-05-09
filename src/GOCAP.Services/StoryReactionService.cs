﻿namespace GOCAP.Services;

[RegisterService(typeof(IStoryReactionService))]
internal class StoryReactionService(
    IStoryReactionRepository _repository,
    IKafkaProducer _kafkaProducer,
    IMapper _mapper,
    ILogger<StoryReactionService> _logger
    ) : ServiceBase<StoryReaction, StoryReactionEntity>(_repository, _mapper, _logger), IStoryReactionService
{
    private readonly IMapper _mapper = _mapper;
    public async Task<OperationResult> CreateOrDeleteAsync(StoryReaction domain)
    {
        var result = new OperationResult(true);
        var storyReaction = await _repository.GetByStoryAndUserAsync(domain.StoryId, domain.UserId);
        if (storyReaction != null)
        {
            _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(StoryReaction).Name);
            var resultDelete = await _repository.DeleteByIdAsync(storyReaction.Id);
            result = new OperationResult(resultDelete > 0);
        }
        else
        {
            _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(StoryReaction).Name);
            domain.InitCreation();
            var entity = _mapper.Map<StoryReactionEntity>(domain);
            await _repository.AddAsync(entity);
            _ = _kafkaProducer.ProduceAsync(KafkaConstants.Topics.Notification, new NotificationEvent
            {
                Type = NotificationType.Reaction,
                ActionType = ActionType.Add,
                ActorId = entity.UserId,
                Source = new NotificationSource
                {
                    Id = entity.StoryId,
                    Type = SourceType.Story,
                }
            });
        }
        return result;
    }

    public async Task<QueryResult<StoryReaction>> GetReactionDetailsWithPagingAsync(Guid storyId, QueryInfo queryInfo)
    {
        var result = await _repository.GetReactionDetailsWithPagingAsync(storyId, queryInfo);
        return _mapper.Map<QueryResult<StoryReaction>>(result);
    }
    
}
