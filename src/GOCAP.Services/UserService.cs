
namespace GOCAP.Services;

[RegisterService(typeof(IUserService))]
internal class UserService(
    IUserRepository _repository,
    IUserNotificationRepository _userNotificationRepository,
    ILogger<UserService> _logger
    ) : ServiceBase<User>(_repository, _logger), IUserService
{
    public override async Task<User> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public override async Task<OperationResult> UpdateAsync(Guid id, User user)
    {
        user.UpdateModify();
        var isSuccess = await _repository.UpdateAsync(id, user);
        return new OperationResult(isSuccess);
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
}
