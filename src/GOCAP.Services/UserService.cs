namespace GOCAP.Services;

[RegisterService(typeof(IUserService))]
internal class UserService(
	IUserRepository _repository,
	IBlobStorageService _blobStorageService,
	IUserNotificationRepository _userNotificationRepository,
	IUserBlockRepository _userBlockRepository,
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
		return await _repository.GetUserCountsAsync() ?? new UserCount();
	}

	public async Task<OperationResult> BlockOrUnBlockAsync(UserBlock model)
	{
		if (model.BlockedUserId == model.BlockedByUserId)
		{
			throw new ParameterInvalidException();
		}

		if (!await _repository.CheckExistAsync(model.BlockedUserId))
		{
			throw new ResourceNotFoundException($"User {model.BlockedUserId} was not found");
		}

		if (!await _repository.CheckExistAsync(model.BlockedByUserId))
		{
			throw new ResourceNotFoundException($"User {model.BlockedByUserId} was not found");
		}

		var block = await _repository.GetBlockOrBlockedAsync(model);
		if (block != null)
		{
			var resultDelete = await _userBlockRepository.DeleteByIdAsync(block.Id);
			return new OperationResult(resultDelete > 0);
		}
		else
		{
			model.InitCreation();
			var blockEntity = _mapper.Map<UserBlockEntity>(model);
			await _userBlockRepository.AddAsync(blockEntity);
			return new OperationResult(true);
		}
	}

}
