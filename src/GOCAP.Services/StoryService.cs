namespace GOCAP.Services;

[RegisterService(typeof(IStoryService))]
internal class StoryService(
    IStoryRepository _repository,
    IUserRepository _userRepository,
    IBlobStorageService _blobStorageService,
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
        story.ExpireTime = DateTime.UtcNow.AddHours(24).Ticks;

        try
        {
            var entity = _mapper.Map<StoryEntity>(story);
            var result = await _repository.AddAsync(entity);
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
}