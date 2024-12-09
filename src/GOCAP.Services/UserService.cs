namespace GOCAP.Services;

[RegisterService(typeof(IUserService))]
internal class UserService(
    IUserRepository _repository,
    ILogger<UserService> _logger
    ) : ServiceBase<User>(_repository, _logger), IUserService
{
    public override async Task<User> AddAsync(User user)
    {
        _logger.LogInformation("Start to a new user");
        user.InitCreation();    
        return await _repository.AddAsync(user);
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
}
