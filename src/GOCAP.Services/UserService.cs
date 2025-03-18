namespace GOCAP.Services;

[RegisterService(typeof(IUserService))]
internal class UserService(
	IUserRepository _repository,
	IBlobStorageService _blobStorageService,
	INotificationRepository _userNotificationRepository,
	IMapper _mapper,
	ILogger<UserService> _logger
	) : ServiceBase<User, UserEntity>(_repository, _mapper, _logger), IUserService
{
	private readonly IMapper _mapper = _mapper;
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
			if (domain.PictureUpload != null)
			{
				var picture = await _blobStorageService.UploadFileAsync(domain.PictureUpload);
				domain.Picture = picture;
			}
			var userEntity = _mapper.Map<UserEntity>(domain);
			var result = await _repository.UpdateAsync(userEntity);
			return new OperationResult(result);
		}
		catch (Exception ex)
		{
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            if (domain.PictureUpload != null)
			{
				_logger.LogInformation("Start deleting media files if occurring errors.");
				await MediaHelper.DeleteMediaFilesIfError([domain.PictureUpload], _blobStorageService);
			}
			throw new InternalException();
		}

	}

	public async Task<bool> IsEmailExists(string email)
	{
		return await _repository.IsEmailExistsAsync(email);
	}

	public async Task<User?> GetByEmailAsync(string email)
	{
		var entity = await _repository.GetByEmailAsync(email);
		return _mapper.Map<User?>(entity);
    }

	public async Task<List<Notification>> GetNotificationsByUserIdAsync(Guid userId)
	{
		return await _userNotificationRepository.GetNotificationsByUserIdAsync(userId);
	}

	public async Task<UserCount> GetUserCountsAsync()
	{
		return await _repository.GetUserCountsAsync() ?? new UserCount();
	}

	public async Task<List<User>> SearchUsersAsync(string userName, int limit)
	{

		return await _repository.SearchUsersAsync(userName, limit);
	}
}
