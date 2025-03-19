using GOCAP.Messaging.Producer;

namespace GOCAP.Services;

[RegisterService(typeof(IStoryService))]
internal class StoryService(
    IStoryRepository _repository,
    IStoryReactionRepository _storyReactionRepository,
    IStoryViewRepository _storyViewRepository,
    IStoryHighlightRepository _storyHighlightRepository,
    IUserRepository _userRepository,
    IBlobStorageService _blobStorageService,
    IUnitOfWork _unitOfWork,
    IKafkaProducer _kafkaProducer,
    IMapper _mapper,
    ILogger<StoryService> _logger
    ) : ServiceBase<Story, StoryEntity>(_repository, _mapper, _logger), IStoryService
{
    private readonly IMapper _mapper = _mapper;

    public override async Task<Story> AddAsync(Story story)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Story).Name);

        if (!await _userRepository.CheckExistAsync(story.UserId))
        {
            throw new ResourceNotFoundException($"User {story.UserId} was not found.");
        }

        // Upload file to azure.
        if (story.MediaUpload is not null)
        {
            var media = await _blobStorageService.UploadFileAsync(story.MediaUpload);
            story.Media = media;
        }

        story.InitCreation();

        try
        {
            var entity = _mapper.Map<StoryEntity>(story);
            var result = await _repository.AddAsync(entity);
            await _kafkaProducer.ProduceAsync(KafkaConstants.Topics.Notification, new NotificationEvent
            {
                Type = NotificationType.Story,
                ActionType = ActionType.Add,
                Source = new NotificationSource
                {
                    Id = result.Id
                },
                ActorId = result.UserId
            });
            return _mapper.Map<Story>(result);
        }
        catch (Exception ex)
        {
            await MediaHelper.DeleteMediaFilesIfError([story.MediaUpload], _blobStorageService);
            throw new InternalException(ex.Message);
        }
    }

    public async Task<QueryResult<Story>> GetFollowingStoriesWithPagingAsync(Guid userId, QueryInfo queryInfo)
    {
        var result = await _repository.GetFollowingStoriesWithPagingAsync(userId, queryInfo);
        return _mapper.Map<QueryResult<Story>>(result);
    }

    public async Task<QueryResult<Story>> GetStoriesByUserIdWithPagingAsync(Guid userId, QueryInfo queryInfo)
    {
        var result = await _repository.GetStoriesByUserIdWithPagingAsync(userId, queryInfo);
        return _mapper.Map<QueryResult<Story>>(result);
    }

    public override async Task<OperationResult> DeleteByIdAsync(Guid id)
    {
        _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(Story).Name);
        var story = await _repository.GetByIdAsync(id);
        var media = JsonHelper.Deserialize<Media>(story.Media);
        if (media != null)
        {
            var isFileDeleted = await _blobStorageService.DeleteFilesByUrlsAsync([media.Url]);
            if (!isFileDeleted)
            {
                return new OperationResult(isFileDeleted, "Unexpected error occurs while deleting media file.");
            }
        }
        try
        {
            var storyHighLight = await _storyHighlightRepository.GetByStoryIdAsync(story.Id, false);
            // Begin transaction by unit of work to make sure the consistency
            await _unitOfWork.BeginTransactionAsync();

            await _storyReactionRepository.DeleteByStoryIdAsync(id);
            await _storyViewRepository.DeleteByStoryIdAsync(id);
            if (storyHighLight != null)
            {
                await _storyHighlightRepository.DeleteAsync(id, storyHighLight.Id);
            }
            var result = await _repository.DeleteByEntityAsync(story);

            // Commit if success
            await _unitOfWork.CommitTransactionAsync();
            return new OperationResult(result > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting entity of type {EntityType}.", typeof(Story).Name);
            // Rollback if fail
            await _unitOfWork.RollbackTransactionAsync();
            return new OperationResult(false);
        }
    }

    public async Task<List<Story>> GetActiveStoriesByUserIdAsync(Guid userId)
    {
        return await _repository.GetActiveStoriesByUserIdAsync(userId);
    }
}