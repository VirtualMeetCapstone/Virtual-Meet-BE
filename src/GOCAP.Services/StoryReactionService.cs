
namespace GOCAP.Services;

[RegisterService(typeof(IStoryReactionService))]
internal class StoryReactionService(
    IStoryReactionRepository _repository,
    IStoryRepository _storyRepository,
    IUserRepository _userRepository,
    ILogger<StoryReactionService> _logger
    ) : ServiceBase<StoryReaction>(_repository, _logger), IStoryReactionService
{
    public async Task<OperationResult> CreateOrDeleteAsync(StoryReaction domain)
    {
        if (!await _storyRepository.CheckExistAsync(domain.StoryId))
        {
            throw new ResourceNotFoundException($"Story {domain.StoryId} was not found.");
        }

        if (!await _userRepository.CheckExistAsync(domain.UserId))
        {
            throw new ResourceNotFoundException($"User {domain.UserId} was not found.");
        }

        var result = new OperationResult(true);
        var roomFavourite = await _repository.GetByStoryAndUserAsync(domain.StoryId, domain.UserId);
        if (roomFavourite != null)
        {
            _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(RoomFavourite).Name);
            var resultDelete = await _repository.DeleteByIdAsync(roomFavourite.Id);
            result = new OperationResult(resultDelete > 0);
        }
        else
        {
            _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(RoomFavourite).Name);
            domain.InitCreation();
            await _repository.AddAsync(domain);
        }
        return result;
    }

    public async Task<QueryResult<StoryReaction>> GetReactionDetailsWithPagingAsync(Guid storyId, QueryInfo queryInfo)
    => await _repository.GetReactionDetailsWithPagingAsync(storyId, queryInfo);
}
