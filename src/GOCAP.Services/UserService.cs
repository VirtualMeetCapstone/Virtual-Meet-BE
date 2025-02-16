namespace GOCAP.Services;

[RegisterService(typeof(IUserService))]
internal class UserService(
    IUserRepository _repository,
    IBlobStorageService _blobStorageService,
    IUserNotificationRepository _userNotificationRepository,
    ILogger<UserService> _logger
    ) : ServiceBase<User>(_repository, _logger), IUserService
{
    public async Task<User> GetUserProfileAsync(Guid id)
    {
        return await _repository.GetUserProfileAsync(id);
    }

    public override async Task<OperationResult> UpdateAsync(Guid id, User domain)
    {
        _logger.LogInformation("Start updating entity of type {EntityType}.", typeof(User).Name);
        domain.UpdateModify();
        try
        {
            var isSuccess = await _repository.UpdateAsync(id, domain);
            return new OperationResult(isSuccess);
        }
        catch (Exception ex)
        {
            if (domain.PictureUpload != null)
            {
                await MediaHelper.DeleteMediaFilesIfError([domain.PictureUpload], _blobStorageService);
            }
            throw new InternalException(ex.Message);
        }
        
    }

    public async Task<bool> IsEmailExists(string email)
    {
        return await _repository.IsEmailExistsAsync(email);
    }

    public Task<User?> GetByEmail(string email)
    {
        return _repository.GetByEmailAsync(email);
    }

    public async Task<List<UserNotification>> GetNotificationsByUserIdAsync(Guid userId)
    {
        return await _userNotificationRepository.GetNotificationsByUserIdAsync(userId);
    }

    public async Task<UserCount> GetUserCountsAsync()
    {
        return await _repository.GetUserCountsAsync()?? new UserCount();
    }
}
