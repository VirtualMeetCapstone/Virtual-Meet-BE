
namespace GOCAP.Services;

[RegisterService(typeof(IStoryService))]
internal class StoryService(
    IStoryRepository _repository,
    IUserRepository _userRepository,
    IBlobStorageService _blobStorageService,
    ILogger<StoryService> _logger
    ) : ServiceBase<Story>(_repository, _logger), IStoryService
{
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
            return await _repository.AddAsync(story);
        }
        catch (Exception ex)
        {
            await MediaHelper.DeleteMediaFilesIfError([story.MediaUpload], _blobStorageService);
            throw new InternalException(ex.Message);
        }
    }

    public async Task<QueryResult<Story>> GetFollowingStoriesWithPagingAsync(Guid userId, QueryInfo queryInfo)
    {
        return await _repository.GetFollowingStoriesWithPagingAsync(userId, queryInfo);
    }
}