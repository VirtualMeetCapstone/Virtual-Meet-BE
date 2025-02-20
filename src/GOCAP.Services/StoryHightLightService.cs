namespace GOCAP.Services;

[RegisterService(typeof(IStoryHightLightService))]
internal class StoryHightLightService(
    IStoryHightLightRepository _repository,
    IStoryRepository _storyRepository,
    IUserRepository _userRepository,
    IMapper _mapper,
    ILogger<StoryHightLightService> _logger
    ) : ServiceBase<StoryHightLight, StoryHightLightEntity>(_repository, _mapper, _logger), IStoryHightLightService
{
    private readonly IMapper _mapper = _mapper;
    public override async Task<StoryHightLight> AddAsync(StoryHightLight domain)
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
        
        domain.InitCreation();

        var entity = _mapper.Map<StoryHightLightEntity>(domain);
        var result = await _repository.AddAsync(entity);
        var story = _mapper.Map<StoryHightLight>(result);

        if (domain.PrevStoryId.HasValue && domain.PrevStoryId != Guid.Empty)
        {
            var prevStory = await _repository.GetByStoryIdAsync(domain.PrevStoryId.Value);
            if (prevStory != null)
            {
                prevStory.NextStoryId = story.Id;
            }
        }

        if (domain.NextStoryId.HasValue && domain.NextStoryId != Guid.Empty)
        {
            var nextStory = await _repository.GetByStoryIdAsync(domain.NextStoryId.Value);
            if (nextStory != null)
            {
                nextStory.PrevStoryId = story.Id;
            }
        }


        return story;
    }
}