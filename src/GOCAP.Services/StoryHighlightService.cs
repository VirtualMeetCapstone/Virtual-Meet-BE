
namespace GOCAP.Services;

[RegisterService(typeof(IStoryHighlightService))]
internal class StoryHighlightService(
    IStoryHighlightRepository _repository,
    IStoryRepository _storyRepository,
    IUserRepository _userRepository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    ILogger<StoryHighlightService> _logger
    ) : ServiceBase<StoryHighlight, StoryHightLightEntity>(_repository, _mapper, _logger), IStoryHighlightService
{
    private readonly IMapper _mapper = _mapper;
    public override async Task<StoryHighlight> AddAsync(StoryHighlight domain)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Story).Name);

        if (await _repository.CheckExistAsync(domain.UserId, domain.StoryId))
        {
            throw new ParameterInvalidException($"Story {domain.StoryId} existed in hight light stories.");
        }

        if (!await _storyRepository.CheckExistAsync(domain.StoryId))
        {
            throw new ResourceNotFoundException($"Story {domain.StoryId} was not found.");
        }

        if (!await _userRepository.CheckExistAsync(domain.UserId))
        {
            throw new ResourceNotFoundException($"User {domain.UserId} was not found.");
        }

        if (await _repository.GetByStoryIdAsync(domain.StoryId) != null)
        {
            throw new ResourceDuplicatedException($"Story {domain.StoryId} existed in hightlight story list");
        }

        domain.InitCreation();
        try
        {
            StoryHightLightEntity? prevStory = null;
            StoryHightLightEntity? nextStory = null;

            if (domain.PrevStoryId.HasValue && domain.PrevStoryId != Guid.Empty)
            {
                prevStory = await _repository.GetByIdAsync(domain.PrevStoryId.Value);

            }

            if (domain.NextStoryId.HasValue && domain.NextStoryId != Guid.Empty)
            {
                nextStory = await _repository.GetByIdAsync(domain.NextStoryId.Value);
            }

            // Begin transaction by unit of work.
            await _unitOfWork.BeginTransactionAsync();

            var entity = _mapper.Map<StoryHightLightEntity>(domain);
            var result = await _repository.AddAsync(entity);
            var story = _mapper.Map<StoryHighlight>(result);

            if (prevStory != null)
            {
                prevStory.NextStoryId = story.Id;
                await _repository.UpdateAsync(prevStory);
            }

            if (nextStory != null)
            {
                nextStory.PrevStoryId = story.Id;
                await _repository.UpdateAsync(nextStory);
            }

            // Commit the transaction if success.
            await _unitOfWork.CommitTransactionAsync();
            return story;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting entity of type {EntityType}.", typeof(StoryHighlight).Name);
            // Rollback the transaction if fail.
            await _unitOfWork.RollbackTransactionAsync();
            throw new InternalException(ex.Message);
        }

    }

    public async Task<OperationResult> DeleteAsync(Guid storyId, Guid storyHighlightId)
    {
        _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(StoryHighlight).Name);
        var result = await _repository.DeleteAsync(storyId, storyHighlightId);
        return new OperationResult(result > 0);
    }

    public async Task<List<List<Story>>> GetStoryHighlightByUserIdAsync(Guid userId)
    => await _repository.GetStoryHighlightByUserIdAsync(userId);
}