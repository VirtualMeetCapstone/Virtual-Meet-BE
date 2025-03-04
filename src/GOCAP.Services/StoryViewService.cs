namespace GOCAP.Services;

[RegisterService(typeof(IStoryViewService))]
internal class StoryViewService(
    IStoryViewRepository _repository,
    IUserRepository _userRepository,
    IStoryRepository _storyRepository,
    IMapper _mapper,
    ILogger<StoryViewService> _logger
    ) : ServiceBase<StoryView, StoryViewEntity>(_repository, _mapper, _logger), IStoryViewService
{
    private readonly IMapper _mapper = _mapper;
    public override async Task<StoryView> AddAsync(StoryView story)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Story).Name);
        if (await _repository.CheckViewerExistAsync(story.StoryId, story.ViewerId))
        {
            throw new ResourceDuplicatedException("This viewer existed.");
        }
        if (!await _userRepository.CheckExistAsync(story.ViewerId))
        {
            throw new ResourceNotFoundException($"User {story.ViewerId} was not found.");
        }
        if (!await _storyRepository.CheckExistAsync(story.StoryId))
        {
            throw new ResourceNotFoundException($"Story {story.StoryId} was not found.");
        }
        story.InitCreation();
        var entity = _mapper.Map<StoryViewEntity>(story);
        var result = await _repository.AddAsync(entity);
        return _mapper.Map<StoryView>(result);
    }

    public async Task<QueryResult<StoryViewDetail>> GetWithPagingAsync(Guid storyId, QueryInfo queryInfo)
    {
        var result = await _repository.GetWithPagingAsync(storyId, queryInfo);
        return _mapper.Map<QueryResult<StoryViewDetail>>(result);
    }
}
