
namespace GOCAP.Services;

[RegisterService(typeof(IUserBlockService))]
internal class UserBlockService
	(IUserBlockRepository _repository,
	IUserRepository _userRepository,
	IMapper _mapper,
	ILogger<UserBlockService> _logger)
	: ServiceBase<UserBlock, UserBlockEntity>(_repository, _mapper, _logger), IUserBlockService
{
	private readonly IMapper _mapper = _mapper;
	public async Task<OperationResult> BlockOrUnblockAsync(UserBlock domain)
	{
		if (domain.BlockedUserId == domain.BlockedByUserId)
		{
			throw new ParameterInvalidException();
		}

		if (!await _userRepository.CheckExistAsync(domain.BlockedUserId))
		{
			throw new ResourceNotFoundException($"User {domain.BlockedUserId} was not found");
		}

		if (!await _userRepository.CheckExistAsync(domain.BlockedByUserId))
		{
			throw new ResourceNotFoundException($"User {domain.BlockedByUserId} was not found");
		}

		var block = await _repository.GetBlockOrBlockedAsync(domain);
		if (block != null)
		{
			var resultDelete = await _repository.DeleteByIdAsync(block.Id);
			return new OperationResult(resultDelete > 0);
		}
		else
		{
			domain.InitCreation();
			var blockEntity = _mapper.Map<UserBlockEntity>(domain);
			await _repository.AddAsync(blockEntity);
			return new OperationResult(true);
		}
	}

    public async Task<UserBlock?> GetBlockOrBlockedAsync(UserBlock domain)
    {
        var result = await _repository.GetBlockOrBlockedAsync(domain);
        return result;
    }

    public async Task<List<UserBlock>> GetUserBlocksAsync(Guid userId)
	{
		var result = await _repository.GetUserBlocksAsync(userId);
		return result;
	}

    public async Task<List<UserBlock>> GetBlockedByUsersAsync(Guid userId)
    {
        return await _repository.GetBlockedByUsersAsync(userId);
    }
}
